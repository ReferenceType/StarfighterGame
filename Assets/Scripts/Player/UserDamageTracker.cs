using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UserDamageTracker : MonoBehaviour
{
    private Rigidbody rb;
    int health = 100;
    public AudioSource Warning;
    public AudioSource MetalHit;
    public Image HealtBar;
    public DeathAnimation DeathAnimation;
    public Action<Guid> ImDead;
    private HashSet<Guid> Ignore =  new HashSet<Guid>();
    public event Action<Guid,int> DamageReceived;
    public event Action Resetting;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool IsDead()
    {
        return health < 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsDead())
            return;

        var bullet = other.GetComponent<BulletCollider>();
        Guid remotePlayerId = Guid.Empty;
        if (bullet != null)
        {
            var dmg = Random.Range(5, 10);
            health -= dmg;
            remotePlayerId = bullet.ParentBullet.RemotePlayerId;
            CameraShaker.Shake(10, override_:true);
            DamageBloodyImage.Draw();
            DamageReceived?.Invoke(remotePlayerId,dmg);
        }
        else
        {
            var missile = other.GetComponent<MissileReference>();
            if (missile != null && !Ignore.Contains(missile.Parent.Id))
            {
                Ignore.Add(missile.Parent.Id);
                var dmg = Random.Range(35, 55);
                health -= dmg;
                remotePlayerId = missile.Parent.remotePlayerId;
                DamageReceived?.Invoke(missile.Parent.remotePlayerId, dmg);
                CameraShaker.Shake(50, 0.02f, 0.02f, override_: true);
                DamageBloodyImage.Draw();

            }
        }
      
        if (health <= 0)
        {
            ImDead?.Invoke(remotePlayerId);
            DeathAnimation.Explode();
        }
        HealtBar.fillAmount = health / 100f;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(IsDead())
            return;
        else
        {
            var hitStrenght = rb.velocity.magnitude; //(int)collision.relativeVelocity.magnitude;
            if(hitStrenght>1)
                DamageBloodyImage.Draw();

            health -= (int)hitStrenght;
            MetalHit.volume= Mathf.Max(0.3f,(Mathf.Clamp01(hitStrenght/3)));
            MetalHit.pitch = Random.Range(0.8f, 1.4f);

            if (!MetalHit.isPlaying)
            {
                MetalHit?.Play();
            }
            else if (MetalHit.time > 1)
                MetalHit.time = 0.2f;
            
            if (hitStrenght > 5 &&!Warning.isPlaying)
            {
                Warning.Play();
            }
            CameraShaker.Shake(20, 0.02f+ hitStrenght/10, 0.004f*hitStrenght/10, override_: true);

        }
        if (health <= 0)
        {
            ImDead?.Invoke(Guid.Empty);
            DeathAnimation.Explode();           
        }
        HealtBar.fillAmount= health/100f;
       
    }
   
    public void ResetHealth() 
    {
        Resetting?.Invoke();
        health = 100;
        HealtBar.fillAmount = health / 100f;
        DeathAnimation.ResetShip();
    }
 
}
