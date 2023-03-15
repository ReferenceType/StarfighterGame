using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletMotion), typeof(ExplodeOnHit))]
public class LaserBullet : MonoBehaviour
{
    public GameObject BulletVisual;
    private BulletMotion motionManager;
    private ExplodeOnHit exploder;
    public BulletPool BulletPool;
    internal Guid Id;
    internal Guid RemotePlayerId;
    public bool IsSelf = true;

    private Collider[] colliders = new Collider[0];
    public AudioSource BulletSound;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(true);
        motionManager = GetComponent<BulletMotion>();
        exploder = GetComponent<ExplodeOnHit>();

        exploder.BulletExploded += OnBulletExploded;
        exploder.BulletExploding += OnBulletExploding;
        motionManager.BulletLifetimeExpired += OnBulletExpired;
    }

    private void OnBulletExpired()
    {
        motionManager.Move = false;
        BulletPool.ReturnBullet(this);
    }

    private void OnBulletExploding()
    {
        Activate(false);
        motionManager.Move = false;
    }

    private void OnBulletExploded()
    {
        BulletPool.ReturnBullet(this);
    }

    internal void Activate(bool v)
    {
        if (BulletSound != null)
        {
            if (v)
            {
                BulletSound.Play();
            }
            else
            {
                BulletSound.Stop();
            }
        }
      
        BulletVisual.SetActive(v);
        foreach (var collider in colliders)
        {
            collider.gameObject.SetActive(v);
        }
    }

    internal void Move()
    {
        motionManager.Move = true;
    }

    internal void SetAsSelfBullet()
    {
        IsSelf = true;
    }
    internal void SetAsExternalBullet()
    {
        IsSelf = false;
    }
    internal void SetPosition(Vector3 vector3)
    { 
        transform.position = vector3;
    }

    internal void SetRotation(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }
}
