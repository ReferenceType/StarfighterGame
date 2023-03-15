using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnHit : MonoBehaviour
{
    public GameObject ExplosionEffectGameObj;
    public ParticleSystem ExplosionEffect;

    internal Action BulletExploding;
    internal Action BulletExploded;
    private bool exploding = false;
    private float counter = -1;

    private void Awake()
    {
        ExplosionEffectGameObj.SetActive(false);
    }
   
    private void FixedUpdate()
    {
        if(counter>0)
            counter--;

        if (exploding && counter == 0 && !ExplosionEffect.isPlaying )
        {
            ExplosionEffectGameObj.SetActive(false);
            BulletExploded?.Invoke();
            exploding = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        ExplosionEffectGameObj.SetActive(true) ;
        ExplosionEffect.Play();
        exploding= true;
        counter = 30;
        BulletExploding?.Invoke();
    }

}
