using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    public LaserBullet ParentBullet;
    void Start()
    {
        ParentBullet = GetComponentInParent<LaserBullet>();
    }

   
}
