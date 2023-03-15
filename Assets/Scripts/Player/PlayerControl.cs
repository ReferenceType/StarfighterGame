using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngineInternal;

public class PlayerControl : MonoBehaviour
{
    public LayerMask AimIgnores;
    public LayerMask RocketIgnores;
    public Vector2 UserInputDirection;
    public Vector2 UserMousePosition;

    public PlayerController inputSystem;
    public ParticleSystem explosionEffect;
    public List<GameObject> bulletSpawnPoints;
    public bool BoosterOn;
    public int MissileRechargeTime = 250;
    public int MissileFocusTime = 50;
    public int BulletRechargeTime = 6;
    public float VelocityMultiplier = 1f;
    public float MissileMaxDist = 500f;
    public GameObject RocketReference;
    public List<GameObject> RocketSpawnReferences;
    public UserDamageTracker DamageTracker;
    int bulletSpawnTimer = 0;
    private BulletPool bulletPool;
    private MissilePool missilePool;
    private SpaceshipAnimator animator;
    private Camera cam;
    private Rigidbody rb;

    public Vector3 StartPosition { get; private set; }

    private Quaternion StartRotation;
    private ParticleSystem flames;
    private float speedMul = 0.01f;
    internal float velocityY;
    private Vector2 inputDirection;
    private float maxSpeed;
    private float minSpeed;
    private float boosterIncrement;
    internal float BoosterLevel = 100;
    Queue<GameObject> bulletCannons = new Queue<GameObject>();
    Queue<GameObject> rocketCannons = new Queue<GameObject>();
    private Dictionary<GameObject, Quaternion> cannonOriginalRotations = new Dictionary<GameObject, Quaternion>();
    private int missileSpawnTimer;
    public AudioSource EngineSound;
    public AudioSource LaserSound;
    public AudioSource MissileLockSound;
    public AudioSource TargetAcquiredSound;
    public Image BoosterBar;


    void Awake()
    {
        inputSystem = new PlayerController();
        inputSystem.Enable();

        bulletPool = FindFirstObjectByType<BulletPool>();
        missilePool = FindFirstObjectByType<MissilePool>();
        animator = GetComponentInChildren<SpaceshipAnimator>();
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        StartPosition = rb.transform.position;
        StartRotation = rb.transform.rotation;
        flames = GetComponentInChildren<ParticleSystem>();

        foreach (var bulletSpawnSorce in bulletSpawnPoints)
        {
            bulletCannons.Enqueue(bulletSpawnSorce);
            cannonOriginalRotations.Add(bulletSpawnSorce, bulletSpawnSorce.transform.rotation);
        }
        foreach (var item in RocketSpawnReferences)
        {
            rocketCannons.Enqueue(item);
        }
        DamageTracker.ImDead += OnPlayerDead;
    }

    private void OnPlayerDead(Guid killer)
    {
        StartCoroutine(ResetDelayed());
    }

    IEnumerator ResetDelayed()
    {
        yield return new WaitForSeconds(5);
        DamageTracker.ResetHealth();
        BoosterLevel = 100f;
        var tr = SpawnPointSelector.GetSpawnPoint();
        transform.position = tr.position; 
        transform.rotation = tr.rotation;
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!CanMove())
            return;
        MoveShip();
        bool gotHit = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000,~AimIgnores))
        {
            gotHit = true;
        }
        
        foreach ( var cannon in bulletSpawnPoints)
        {
            if (gotHit)
            {
                cannon.transform.LookAt(hit.point);
            }
            else
            {
                cannon.transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
            } 
        }
        
        var Lfire = inputSystem.Player.Fire.ReadValue<float>();
        if (Lfire > 0)
        {
            FireBullet();
        }
        gotHit = false;
        if (Physics.Raycast(ray, out hit, 10000, ~RocketIgnores))
        {
            if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Environment")
                && this.transform.InverseTransformPoint(hit.collider.transform.position).magnitude<MissileMaxDist)
                gotHit = true;
        }
        MissileFocusBar.Progress01 =  consesutiveMissileHits/ (float)MissileFocusTime;
        MisssileRechargeBar.Progress01 =  missileSpawnTimer/(float)MissileRechargeTime;
        var Rfire = inputSystem.Player.RocketFire.ReadValue<float>();
        if (gotHit && Rfire > 0)
        {
            FireMissile(hit);
        }
        else
        {
            //if (consesutiveMissileHits == 0 && MissileLockSound.isPlaying)
            //    MissileLockSound.Stop();
            if (consesutiveMissileHits > 0)
                consesutiveMissileHits= Mathf.Max(0, consesutiveMissileHits-4);
        }

        if (bulletSpawnTimer > 0)
        {
            bulletSpawnTimer--;
        }
        if (missileSpawnTimer > 0)
        {
            missileSpawnTimer--;
        }
    }

    private bool CanMove()
    {
       bool isDead =  DamageTracker.IsDead();
        if (isDead)
        {
            velocityY= 0;
            speedMul = 0;
            BoosterOn = false;
            EngineSound.pitch = Mathf.Min(velocityY * speedMul * 40 + 0.5f, 2.2f);
        }
        return !isDead;
    }

    int consecutiveCount = 0;
    bool boosterTransition = false;
    void MoveShip()
    {
        EngineSound.pitch = Mathf.Min(velocityY * speedMul * 40 + 0.5f, 2.2f);
        BoosterOn = inputSystem.Player.SpeedBoost.ReadValue<float>() > 0;
        if (BoosterOn)
        {
            if (boosterTransition)
            {
                BoosterLevel -= 1;
                boosterTransition= false;
            }
                
            consecutiveCount = 0;

            BoosterLevel -= 0.5f*velocityY;
            if(BoosterLevel<0)
                BoosterOn =false;
        }
        else
        {
            boosterTransition = true;
            consecutiveCount++;
            BoosterLevel = Math.Clamp(BoosterLevel +(0.001f * consecutiveCount / Mathf.Max(0.2f,velocityY)), 0, 100f);
        }

        BoosterBar.fillAmount= BoosterLevel/100f;
        inputDirection = inputSystem.Player.Move.ReadValue<Vector2>();
        UserInputDirection = new Vector2(inputDirection.x, inputDirection.y);
        inputDirection.Normalize();

        CalculateVelocity(inputDirection);

        rb.MovePosition(this.transform.localPosition + this.transform.forward * velocityY * VelocityMultiplier);
        animator.MoveForward(velocityY * 2);

        mousePosition = inputSystem.Player.Look.ReadValue<Vector2>();
        UserMousePosition = new Vector2(mousePosition.x, mousePosition.y);
        var port = cam.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        var x = port.x;
        var y = port.y;
        var center = new Vector2(x, y);

        mousePosition = new Vector2(mousePosition.x - center.x, mousePosition.y - center.y);
        var corrected = new Vector2(-mousePosition.y, mousePosition.x);
        var dx = Mathf.Abs(corrected.x) > Screen.width / 30 ? corrected.x : 0;
        var dy = Mathf.Abs(corrected.y) > Screen.height / 30 ? corrected.y : 0;
        corrected = new Vector2(dx, dy);
        corrected *= new Vector2(0.001f, 0.001f);

        corrected = new Vector2(corrected.x * 3840 / Screen.width, corrected.y*2160 / Screen.height);
        //Debug.Log(corrected);
        
        rb.MoveRotation(transform.localRotation * Quaternion.Euler(new Vector3(corrected.x, corrected.y, inputDirection.x)));
        animator.RotateXY(corrected.y * 0.1f, corrected.x * 0.1f, inputDirection.x * 0.3f);
    }

    void FireBullet()
    {
        if (bulletSpawnTimer == 0)
        {
            bulletSpawnTimer = BulletRechargeTime;
            var bulletReference = bulletCannons.Dequeue();
            var bullet = bulletPool.GetBullet();
            bullet.SetPosition(bulletReference.transform.position);
            bullet.SetRotation(bulletReference.transform.rotation);
            bullet.Activate(true);
            bullet.Move();
            LaserSound.panStereo = LaserSound.panStereo >= 0 ? -0.5f : 0.5f;
            LaserSound.Play();
            bulletCannons.Enqueue(bulletReference);

        }
    }

    int consesutiveMissileHits = 0;
    private Vector2 mousePosition;
    void FireMissile(RaycastHit hit)
    {
        if (missileSpawnTimer == 0)
        {
            if (consesutiveMissileHits < MissileFocusTime)
            {
                consesutiveMissileHits++;
                Player.SendTargettedMessage(hit.rigidbody.gameObject.GetComponent<RemotePlayer>());
                if(!MissileLockSound.isPlaying)
                    MissileLockSound.Play();
                return;
            }
            consesutiveMissileHits = 0;
            MissileLockSound.Stop();
            TargetAcquiredSound.Play();
            missileSpawnTimer = MissileRechargeTime;
            var RocketSpawnReference = rocketCannons.Dequeue();

            var missile = missilePool.GetMissile();
            missile.SetPosition(RocketSpawnReference.transform.position);
            missile.SetRotation(RocketSpawnReference.transform.rotation);

            Guid.TryParse(hit.collider.gameObject.name, out Guid id);
            missile.Activate(true,id);

            missile.Target = hit.collider.gameObject;
            missile.seekTarget = true;
            rocketCannons.Enqueue(RocketSpawnReference);

        }  
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
            maxSpeed = Mathf.Min(maxSpeed+0.02f, 0.60f);
            minSpeed = -0.01f;
            boosterIncrement = 0.002f;
            speedMul = Math.Max(0.040f, speedMul);

        }

        if (direction.y > 0)
        {
            speedMul = Mathf.Min(0.1f, speedMul + speedMul * speedMul * boosterIncrement);
            velocityY += speedMul * direction.y * (maxSpeed - velocityY) * velocityY + 0.002f;
            flames.startSpeed = velocityY * 2;
        }
        else if (direction.y < 0)
        {
            speedMul = Mathf.Max(0.05f, speedMul - speedMul * speedMul * 0.04f);
            velocityY += direction.y * 0.004f;
            flames.startSpeed = velocityY * speedMul * 2;

        }
        else //0
        {
            speedMul = Mathf.Max(0.05f, speedMul - speedMul * speedMul * 0.04f);
            velocityY -= 0.002f * velocityY;
            if (velocityY < 0)
                velocityY -= 0.02f * velocityY;
            flames.startSpeed = velocityY * speedMul * 2;


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
