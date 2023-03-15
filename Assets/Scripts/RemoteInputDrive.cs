using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RemoteInputDrive : MonoBehaviour
{
    private bool boosterTransition;
    private float BoosterLevel = 100;
    private int consecutiveCount;
    private float velocityY;
    private Vector2 inputDirection;
    private Vector2 remoteInputDirection;
    private Rigidbody rb;
    private Vector2 mousePosition;
    private Vector2 RemotemousePosition;
    private float VelocityMultiplier = 2f;
    private float maxSpeed;
    private float minSpeed;
    private float boosterIncrement;
    private float speedMul = 0.01f;
    private ParticleSystem flames;


    public Vector2 KeyboardInputDirection;
    public Vector2 MousePosition;
    public bool BoosterOn;

    public SpaceshipAnimator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        remoteInputDirection = KeyboardInputDirection;
        RemotemousePosition= MousePosition;
        MoveShip();
        
    }

    void MoveShip()
    {
        //EngineSound.pitch = Mathf.Min(velocityY * speedMul * 40 + 0.5f, 2.2f);
        if (BoosterOn)
        {
            if (boosterTransition)
            {
                BoosterLevel -= 1;
                boosterTransition = false;
            }

            consecutiveCount = 0;

            BoosterLevel -= 0.5f * velocityY;
            if (BoosterLevel < 0)
                BoosterOn = false;
        }
        else
        {
            boosterTransition = true;
            consecutiveCount++;
            BoosterLevel = System.Math.Clamp(BoosterLevel + (0.001f * consecutiveCount / Mathf.Max(0.2f, velocityY)), 0, 100f);
        }

        inputDirection = remoteInputDirection;
        inputDirection.Normalize();

        CalculateVelocity(inputDirection);

        rb.MovePosition(this.transform.localPosition + this.transform.forward * velocityY * VelocityMultiplier);
        animator.MoveForward(velocityY * 2);

        var port = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        var x = port.x;
        var y = port.y;
        var center = new Vector2(x, y);

        mousePosition = RemotemousePosition;
        mousePosition = new Vector2(mousePosition.x - center.x, mousePosition.y - center.y);
        var corrected = new Vector2(-mousePosition.y, mousePosition.x);
        var dx = Mathf.Abs(corrected.x) > Screen.width / 30 ? corrected.x : 0;
        var dy = Mathf.Abs(corrected.y) > Screen.height / 30 ? corrected.y : 0;
        corrected = new Vector2(dx, dy);
        corrected *= new Vector2(0.001f, 0.001f);

        corrected = new Vector2(corrected.x * 3840 / Screen.width, corrected.y * 2160 / Screen.height);
        Debug.Log(corrected);

        rb.MoveRotation(transform.localRotation * Quaternion.Euler(new Vector3(corrected.x, corrected.y, inputDirection.x)));
        animator.RotateXY(corrected.y * 0.1f, corrected.x * 0.1f, inputDirection.x * 0.3f);
    }

    private void CalculateVelocity(Vector2 direction)
    {
        if (!BoosterOn)
        {
            if (maxSpeed > 0.25f)
            {
                maxSpeed -= 0.0040f;
                if (velocityY > maxSpeed)
                    velocityY -= 0.0040f;
            }
            else
                maxSpeed = 0.25f;

            minSpeed = -0.01f;
            boosterIncrement = 0.002f;
        }
        else
        {
            maxSpeed = Mathf.Min(maxSpeed + 0.02f, 0.60f);
            minSpeed = -0.01f;
            boosterIncrement = 0.002f;
            speedMul = System.Math.Max(0.040f, speedMul);

        }

        if (direction.y > 0)
        {
            speedMul = Mathf.Min(0.1f, speedMul + speedMul * speedMul * boosterIncrement);
            velocityY += speedMul * direction.y * (maxSpeed - velocityY) * velocityY + 0.002f;
            //flames.startSpeed = velocityY * 2;
        }
        else if (direction.y < 0)
        {
            speedMul = Mathf.Max(0.05f, speedMul - speedMul * speedMul * 0.04f);
            velocityY += direction.y * 0.004f;
            //flames.startSpeed = velocityY * speedMul * 2;

        }
        else //0
        {
            speedMul = Mathf.Max(0.05f, speedMul - speedMul * speedMul * 0.04f);
            velocityY -= 0.002f * velocityY;
            if (velocityY < 0)
                velocityY -= 0.02f * velocityY;
           // flames.startSpeed = velocityY * speedMul * 2;


        }
        if (velocityY < minSpeed)
        {
            velocityY = minSpeed;
        }
        if (velocityY > maxSpeed)
        {

            velocityY = maxSpeed;
        }
        return;

    }
}
