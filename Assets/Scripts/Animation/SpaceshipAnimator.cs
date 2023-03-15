using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceshipAnimator : MonoBehaviour
{
    float maxForwardMotion = 2;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Rigidbody rb;
    public bool debug = false;
    float counter=0;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = this.transform.localPosition;
        initialRot = this.transform.localRotation;
        rb = this.GetComponentInChildren <Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            counter += 0.01f;
            //MoveForward(counter);
           // RotateX(counter);
        }
    }

    public void MoveForward(float percentage)
    {
        percentage = Mathf.Clamp(percentage,-0.5f,1);
        this.transform.localPosition = percentage*5 * Vector3.forward + initialPos;
    }

    public void RotateXY(float percentageX,float percentageY, float percentageZ)
    {
        percentageX = Mathf.Clamp(percentageX,-1,1);
        percentageY = Mathf.Clamp(percentageY,-1,1);
        //rb.MoveRotation(initialRot * Quaternion.AngleAxis(percentageX * 90, Vector3.up) *
        //    Quaternion.AngleAxis(percentageY * 90, Vector3.right));

        var rot = initialRot * 
            Quaternion.AngleAxis(percentageX * 90, Vector3.up) *
            Quaternion.AngleAxis(percentageY * 90, Vector3.right)*
            Quaternion.AngleAxis(percentageZ * 90, Vector3.forward);
        //this.transform.localRotation = rot;
        this.transform.localRotation = Quaternion.Slerp(transform.localRotation, rot, 0.1f);
        //rb.MoveRotation(Quaternion.Slerp(transform.localRotation, rot, 0.1f));
    }
    public void RotateY(float percentage)
    {
        percentage = Mathf.Clamp(percentage, -1, 1);
        this.transform.localRotation = initialRot * Quaternion.AngleAxis(percentage * 90, Vector3.left);
    }

}
