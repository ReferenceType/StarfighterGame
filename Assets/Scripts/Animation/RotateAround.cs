using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public Vector3 Rotation = new Vector3(0, 0, 0.1f);
    void FixedUpdate()
    {
        this.transform.Rotate(Rotation);
    }
}
