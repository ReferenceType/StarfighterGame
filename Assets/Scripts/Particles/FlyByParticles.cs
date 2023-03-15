using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FlyByParticles : MonoBehaviour
{
    private ParticleSystem particles;
    private PlayerControl controller;
    protected ParticleSystem.MainModule flybyParticleSystemMainModule;

    protected virtual void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        flybyParticleSystemMainModule = particles.main;
        controller = FindAnyObjectByType<PlayerControl>();
    }


   
    public virtual void UpdateEffect()
    {
        
    }

    private void Update()
    {
        if (controller.velocityY < 0.02f)
        {
            particles.Stop();
            return;
        }
        else
            particles.Play();

        if (controller.BoosterOn)
        {
            flybyParticleSystemMainModule.simulationSpeed = 7 * controller.velocityY;
            var vv = particles.velocityOverLifetime;
            vv.z = 100 * controller.velocityY * 2;
            flybyParticleSystemMainModule.startSize = controller.velocityY;

            var e = particles.emission;
            e.rateOverTime = 30*controller.velocityY;
            //CameraShaker.Shake(1, 0.01f*controller.velocityY, 0.002f*controller.velocityY);
        }
        else
        {
            var vv = particles.velocityOverLifetime;
            vv.z = 100 * controller.velocityY * 2;

            var e = particles.emission;
            e.rateOverTime = 30 * controller.velocityY;

            flybyParticleSystemMainModule.simulationSpeed = Mathf.Max(0.2f,7 * controller.velocityY);
            flybyParticleSystemMainModule.startSize = controller.velocityY / 3;
        }

    }
}
