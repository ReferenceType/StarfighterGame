using Assets;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MissilePool : MonoBehaviour
{
    private ConcurrentDictionary<Guid, HeatSeekerMissile> localActiveMissiles = new ConcurrentDictionary<Guid, HeatSeekerMissile>();
    private ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, HeatSeekerMissile>> remoteActiveMissiles
        = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, HeatSeekerMissile>>();

    public GameObject LocalMissileReference;
    public GameObject RemoteMissileReference;

    HashSet<Guid> ignore = new HashSet<Guid>();

    
    public HeatSeekerMissile GetMissile()
    {
        var missile = Instantiate(LocalMissileReference).GetComponent<HeatSeekerMissile>();
        Guid guid = Guid.NewGuid();
        missile.Id = guid;
        localActiveMissiles.TryAdd(guid, missile);
        return missile;
    }

    public void ReturnMissile(HeatSeekerMissile missile)
    {
        localActiveMissiles.TryRemove(missile.Id, out var id);
        if (remoteActiveMissiles.TryRemove(missile.remotePlayerId, out var missleDict))
        {
            missleDict.TryRemove(missile.Id, out _);
        }
    }

    internal Dictionary<Guid, MissileState> GetMissileStates()
    {
        var states = new Dictionary<Guid, MissileState>();
        foreach (var missile in localActiveMissiles)
        {
            var missileCoord = new TransformState(missile.Value.transform.position, missile.Value.transform.rotation);
            var targetCoord = new TransformState(missile.Value.Target.transform.position, missile.Value.Target.transform.rotation);
            var missileState = new MissileState()
            {
                missileCoordinates = missileCoord,
                targetCoordinates = targetCoord,
                MissileId = missile.Key,
                TargetId = missile.Value.missileTargetId,
                targetName = EnvironmentObjectTracker.sceneObjectsByObject[missile.Value.Target],
            };
            states.Add(missile.Key, missileState);
        }
        return states;
    }

    internal void UpdateMissiles(Guid remotePlayerId, Dictionary<Guid, MissileState> missileStates)
    {
        if (missileStates == null)
            return;
        if (!remoteActiveMissiles.TryGetValue(remotePlayerId, out var remoteMissileDict))
        {
            remoteMissileDict = new ConcurrentDictionary<Guid, HeatSeekerMissile>();
            remoteActiveMissiles.TryAdd(remotePlayerId, remoteMissileDict);
        }
        foreach (var missileState in missileStates)
        {
            if (!remoteMissileDict.TryGetValue(missileState.Key, out var missile))
                SpawnExternalMissile(missileState.Value, missileState.Key, remotePlayerId);
            else
                missile.UpdateState(missileState.Value);

        }
    }
    // missile id come null here???
    public void SpawnExternalMissile(MissileState missileState, Guid missleId, Guid playerId)
    {
        if (ignore.Contains(missileState.MissileId))
            return;
        ignore.Add(missileState.MissileId);

        if (!remoteActiveMissiles.TryGetValue(playerId, out var bulletDict))
        {
            return;
        }

        var missile = Instantiate(RemoteMissileReference).GetComponent<HeatSeekerMissile>();
        missile.Id = missleId;
        missile.remotePlayerId = playerId;

        bulletDict.TryAdd(missleId, missile);
        var p = missileState.missileCoordinates;
        missile.SetPosition(new Vector3(p.posX, p.posY, p.posZ));
        missile.SetRotation(new Quaternion(p.rotX, p.rotY, p.rotZ, p.rotW));
        missile.Activate(true,missileState.TargetId,2);
        missile.Target = EnvironmentObjectTracker.sceneObjectsById[missileState.targetName];
        if (missileState.targetName.Equals(Player.sessionId))
        {
            missile.PlayerIsTargetted();
        }

        missile.seekTarget = true;
    }

}

