using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    Vector3 originalPos;
    Quaternion originalRot;
    public static int remainingDuration = 0;
    public static float intensity = 0.01f;
    public static float intensityR = 0.002f;
    void Start()
    {
        originalPos = this.transform.localPosition;
        originalRot = this.transform.localRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (remainingDuration > 0)
        {
            remainingDuration--;
            transform.localPosition = new Vector3(originalPos.x + Random.Range(0, 1 * intensity), originalPos.y + Random.Range(0, 1 * intensity), originalPos.z + Random.Range(0, 1 * intensity));
            transform.localRotation = new Quaternion(originalRot.x + Random.Range(0, 1 * intensityR), originalRot.y + Random.Range(0, 1 * intensityR), originalRot.z + Random.Range(0, 1 * intensityR), originalRot.w + Random.Range(0, 1 * intensityR));
        }
        else
        {
            transform.localPosition = originalPos;
            transform.localRotation = originalRot;
        }
    }

    public static void Shake(int duration, float intensityPos = 0.01f, float intensityRot = 0.002f, bool override_ = false)
    {
        if (remainingDuration > 0)
        {
            if (override_)
                intensity = intensityPos;
        }
        else
            intensity = intensityPos;

        if (remainingDuration > 0)
        {
            if (override_)
                intensityR = intensityRot;
        }
        else
            intensityR = intensityRot;

        if (remainingDuration > 0)
        {
            if (override_)
                remainingDuration = duration;

        }
        else
            remainingDuration = duration;


    }
}
