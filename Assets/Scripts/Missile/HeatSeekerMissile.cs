using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class HeatSeekerMissile : MonoBehaviour
{
    public GameObject Target;
    public GameObject MissileVisual;
    public GameObject ExplosionEffect;
    public ParticleSystem jet;
    public int MaxLifetime = 1500;
    public MissilePool MissilePool;
    public bool seekTarget;
    public float Speed = 0.01f;
    public AudioSource ExplosionSound;
    public AudioSource ExplosionSound1;
    public AudioSource JetSound;
    public AudioSource TargettedSound;
    public Action<HeatSeekerMissile> OnExploded;
    public TrailRenderer MissileTrail;
    public Rigidbody Rigidbody;

    internal Guid remotePlayerId;
    internal Guid missileTargetId;
    public Guid Id { get; internal set; }

    private Vector3 targetPos;
    private int warmUpTotalIterations;
    private int currentLifetime;
    private int warmTime = 20;

    void Awake()
    {
        warmUpTotalIterations = 20;
    }
    
    private void FixedUpdate()
    {
        try
        {
            if (seekTarget)
            {
                currentLifetime++;
                if (currentLifetime >= MaxLifetime)
                {
                    Explode();
                    return;
                }

                if (IsTargetDead())
                {
                    Explode();
                    return;
                }

                if (WarmUp())
                    return;

                transform.LookAt(Target.transform);
                var targetpos = Rigidbody.position + transform.forward * Speed;
                Rigidbody.MovePosition(targetpos);
            }
           
        }
        catch
        {
            Explode();
        }
       
    }

    private bool IsTargetDead()
    {
       return Player.Instance.IsPlayerDead(missileTargetId);
    }

    private bool WarmUp()
    {
        if (warmTime == warmUpTotalIterations)
        {
            MissileTrail.emitting = true;
            JetSound.Play();
        }
        if (warmTime > 1)
        {
            warmTime--;
            var targetpos = Rigidbody.position + transform.forward * 0.4f * 10 / warmTime;
            Rigidbody.MovePosition(targetpos);
            if (warmTime < 2)
            {
                if(jet!=null)
                    jet.startSpeed = 0.5f;
            }
            return true;
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        if (TargettedSound != null)
            TargettedSound.Stop();
        if(ExplosionSound!=null)
            ExplosionSound.Play();
        if(ExplosionSound1)
            ExplosionSound.Play();
        if (jet != null)
            jet.startSpeed = 0;

        seekTarget = false;
        warmTime = warmUpTotalIterations;
        currentLifetime = 0;

        MissileVisual.SetActive(false);
        ExplosionEffect.SetActive(true);

        MissileTrail.emitting = false;
        StartCoroutine(SelfDestruct());
        OnExploded?.Invoke(this);
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5);
        ExplosionEffect.SetActive(false);
        Target = null;
        MissilePool.ReturnMissile(this);
        GameObject.Destroy(this.gameObject);
    }

    internal void Activate(bool v, Guid targetId, int skipAheadFrameCnt = 0)
    {
        missileTargetId = targetId;
        if (v)
        {
            MissileVisual.SetActive(true);
            if (jet != null)
                jet.startSpeed = 0.5f;
            warmTime -= skipAheadFrameCnt;
            warmUpTotalIterations -= skipAheadFrameCnt;
        }
        else
        {
            MissileVisual.SetActive(false);
            if (jet != null)
                jet.startSpeed = 0f;
            seekTarget= false;
        }
       
    }

    internal void SetPosition(Vector3 pos)
    {
        this.transform.position = pos;
    }

    internal void SetRotation(Quaternion rot)
    {
       this.transform.rotation = rot;
    }

    internal void UpdateState(MissileState missileState)
    {
        
    }

    public void PlayerIsTargetted()
    {
        if (TargettedSound != null)
            TargettedSound.Play();
    }
}
