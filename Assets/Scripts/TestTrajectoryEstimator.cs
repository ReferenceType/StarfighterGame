using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrajectoryEstimator : MonoBehaviour
{
    public GameObject predictorBody;
    private Player player;
    public float timeToReach = 2;

    private Rigidbody rb;
    public Vector3 deltaX;
    public Vector3 velocity;
    Vector3 posLast =  Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        predictorBody = Instantiate(predictorBody);
        player = FindFirstObjectByType<Player>();
    }
    
    private void FixedUpdate()
    {
        timeToReach =GetTimeToHit();
        deltaX = rb.position - posLast;
        velocity = 3*velocity+ deltaX / Time.fixedDeltaTime;
        velocity /= 4;
        posLast = rb.position;

        var goal = rb.position + velocity * timeToReach;
        predictorBody.transform.position = Vector3.Lerp(predictorBody.transform.position, goal, Time.fixedDeltaTime*2);
    }

    private float GetTimeToHit()
    {
        var dist = player.transform.InverseTransformPoint(transform.position);
        return dist.magnitude/300;
    }

    private void OnDestroy()
    {
        Destroy(predictorBody);   
    }

}
