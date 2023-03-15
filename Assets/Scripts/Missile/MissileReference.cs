using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileReference : MonoBehaviour
{
    public HeatSeekerMissile Parent;
    void Start()
    {
        Parent = GetComponentInParent<HeatSeekerMissile>();
    }

  
}
