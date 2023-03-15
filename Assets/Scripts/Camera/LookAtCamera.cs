using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _camera;
    private void Start()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        transform.LookAt(_camera.transform, Vector3.up);
    }
}
