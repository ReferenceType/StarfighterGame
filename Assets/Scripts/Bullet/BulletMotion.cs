using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMotion : MonoBehaviour
{
    public Vector3 direction = new Vector3(0, 1f, 0);
    public Rigidbody Rbody;
    private bool move;
    private Vector3 distanceTraveled;
    internal Action BulletLifetimeExpired;

    public bool Move { get => move; set { move = value; distanceTraveled = Vector3.zero;}}

    void Start()
    {
        distanceTraveled = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (Move)
        {
            Rbody.MovePosition(this.transform.localPosition + this.transform.forward * direction.magnitude);
            distanceTraveled += direction;
        }
        if (distanceTraveled.magnitude > 500)
        {
            Move = false;
            distanceTraveled = Vector3.zero;
            BulletLifetimeExpired?.Invoke();
        }

    }

    private void OnDisable()
    {
        distanceTraveled = Vector3.zero;
    }
    private void OnEnable()
    {
        distanceTraveled = Vector3.zero;
    }

}
