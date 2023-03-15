using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class PosRot
{
    public Vector3 pos;
    public Quaternion rot;

    public PosRot(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }
}
public class DeathAnimation : MonoBehaviour
{
    public GameObject spaceship;
    public GameObject ExplosionEffect;
    public AudioSource ExplosionSound;
    private Transform[] parts;
    private Dictionary<Transform,PosRot> originalCoord =  new Dictionary<Transform, PosRot> { };

    private bool boom;

    void Start()
    {
        parts = spaceship.GetComponentsInChildren<Transform>();
        foreach (Transform part in parts)
        {
            originalCoord[part] = new PosRot(part.localPosition, part.localRotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (boom)
        {
            foreach (Transform part in parts)
            {
                part.localPosition = Vector3.Lerp(part.localPosition, part.localPosition + part.forward*Random.Range(2f,20f), Time.deltaTime);
            }
        }
       
    }

    public void Explode()
    {
        boom = true;
        ExplosionEffect.SetActive(true);
        ExplosionSound.Play();
        foreach (Transform part in parts)
        {
            part.localRotation =  Quaternion.Euler(Random.Range(2f, 360f), Random.Range(2f, 360f), Random.Range(2f, 360f));
        }
    }

    public void ResetShip()
    {
        boom = false;
        ExplosionEffect.SetActive(false);
        foreach (Transform part in parts)
        {
            part.transform.localRotation = originalCoord[part].rot;
            part.transform.localPosition = originalCoord[part].pos;
        }
    }
}
