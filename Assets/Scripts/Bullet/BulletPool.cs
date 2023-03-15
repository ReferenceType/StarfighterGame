using Assets;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private ConcurrentBag<LaserBullet> selfInactiveBullets = new ConcurrentBag<LaserBullet>();
    private ConcurrentBag<LaserBullet> externalBulletBag = new ConcurrentBag<LaserBullet>();

    private ConcurrentDictionary<Guid, LaserBullet> activeBullets = new ConcurrentDictionary<Guid, LaserBullet>();
    private ConcurrentDictionary<Guid, ConcurrentDictionary<Guid,LaserBullet>> activeExternalBullets =  new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, LaserBullet>>();

    public GameObject SelfBulletReference;
    public GameObject ExternalBulletReference;

    public void SpawnExternalBullet(TransformState bulletCoordinates, Guid bulletId, Guid playerId)
    {
        if (!externalBulletBag.TryTake(out var bullet))
        {
            bullet = Instantiate(ExternalBulletReference).GetComponent<LaserBullet>();
           
        }
        bullet.Id = bulletId;
        bullet.RemotePlayerId = playerId;
        bullet.SetAsExternalBullet();

        if (!activeExternalBullets.TryGetValue(playerId, out var bulletDict))
        {
            return;
        }
        bulletDict.TryAdd(bulletId, bullet);

        bullet.SetPosition(new Vector3(bulletCoordinates.posX, bulletCoordinates.posY, bulletCoordinates.posZ));
        bullet.SetRotation(new Quaternion(bulletCoordinates.rotX, bulletCoordinates.rotY, bulletCoordinates.rotZ, bulletCoordinates.rotW));
        bullet.Activate(true);
        bullet.Move();
    }

    public LaserBullet GetBullet()
    {
        if (selfInactiveBullets.TryTake(out LaserBullet bullet))
        {
            bullet.Id = Guid.NewGuid();
            activeBullets.TryAdd(bullet.Id, bullet);
            bullet.Activate(true);
        }
        else
        {
            bullet = Instantiate(SelfBulletReference).GetComponent<LaserBullet>();
            bullet.SetAsSelfBullet();
            Guid guid = Guid.NewGuid();
            bullet.Id = guid;
            activeBullets.TryAdd(guid, bullet);
        }
        return bullet;
    }

    public void ReturnBullet(LaserBullet bullet)
    {
        bullet.Activate(false);
        bullet.SetPosition(Vector3.zero);
        bullet.SetRotation(Quaternion.identity);

        if (activeBullets.TryRemove(bullet.Id, out _))
        {
            selfInactiveBullets.Add(bullet);
        }
        else if (activeExternalBullets.TryGetValue(bullet.RemotePlayerId, out var bulletDict))
        {
            if (bulletDict.TryRemove(bullet.Id, out _))
            {
               
                if(externalBulletBag.Count<100)
                    externalBulletBag.Add(bullet);
                else
                {
                    Destroy(bullet);
                }
            }
        }
    }

    internal void UpdateBullets(Guid remotePlayerId, Dictionary<Guid, TransformState> bulletStates)
    {
        if (!activeExternalBullets.TryGetValue(remotePlayerId, out var remoteBulletDict))
        {
            remoteBulletDict = new ConcurrentDictionary<Guid, LaserBullet>();
            activeExternalBullets.TryAdd(remotePlayerId, remoteBulletDict);
        }
        foreach (var item in bulletStates)
        {
            if(!remoteBulletDict.ContainsKey(item.Key))
                SpawnExternalBullet(item.Value, item.Key, remotePlayerId);

        }
    }

    internal Dictionary<Guid, TransformState> GetBulletStates()
    {
        var states = new Dictionary<Guid, TransformState>();
        foreach (var item in activeBullets)
        {
            states.Add(item.Key, new TransformState(item.Value.transform.position, item.Value.transform.rotation));
        }
        return states;
    }
}
