using Assets;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class RemotePlayer : MonoBehaviour
{
    SortedDictionary<ulong, PlayerState> stateBuffer = new SortedDictionary<ulong, PlayerState>();
    ConcurrentQueue<PlayerState> snapshotQueue = new ConcurrentQueue<PlayerState>();

    internal DateTime lastTimestamp = DateTime.UtcNow.AddDays(-1);
    Vector3 currentPos = Vector3.zero;
    Quaternion currentRot = Quaternion.identity;
    Vector3 prevPos = Vector3.zero;
    Quaternion prevRot = Quaternion.identity;
    private Rigidbody rb;

    bool begin = false;
    private int jitterBufferLenght = 0;
    const int maxJitterLenght = 0;
    internal ulong lastSequence;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.isKinematic= true;
        prevPos = rb.position;
        prevRot = rb.rotation;
        lastPosD = rb.position;
        lastR = rb.rotation;
        bulletPool = FindObjectOfType<BulletPool>();
    }

    // Update is called once per frame
    ulong seqProcessed = 0;
    Vector3 lastPosDelta = Vector3.zero;
    Quaternion lastRotDelta =Quaternion.identity;
    Vector3 lastPosD = Vector3.zero;
    Quaternion lastR = Quaternion.identity;
    private BulletPool bulletPool;

    void FixedUpdate()
    {
        //Strategy4();
        //return
        //Strategy3();
        //return;
        //Strategy2();
        //return;


        if (!begin)
            return;

        while (snapshotQueue.Count > jitterBufferLenght+1)
        {
            snapshotQueue.TryDequeue(out var staleState);

            if (staleState.bulletStates != null)
                bulletPool.UpdateBullets(staleState.PlayerId, staleState.bulletStates);

            if (seqProcessed >= staleState.sequenceNumber)
                continue;
            //CosnumeSnapshot(staleState);
            seqProcessed++;
        }
        if (snapshotQueue.TryDequeue(out var state))
        {
            if (state.bulletStates != null)
                bulletPool.UpdateBullets(state.PlayerId, state.bulletStates);

            //if (seqProcessed >= state.sequenceNumber)
            //    return;

            CosnumeSnapshot(state);
            seqProcessed++;
        }
        else
        {
           // Extrapolate();
            seqProcessed++;
        }
        lastPosDelta = rb.position - lastPosD;
        lastRotDelta = rb.rotation * Quaternion.Inverse(lastR);
        lastPosD = rb.position;
        lastR = rb.rotation;
        //this.transform.position = currentPos;
        //this.transform.rotation = currentRot;
        //if (snapshotReceived)
        //{
        //    snapshotReceived = false;
        //    rb.MovePosition(currentPos);
        //    rb.MoveRotation(currentRot);

        //}
        //else
        //{
        //    var pos = transform.position + currentPos - prevPos;
        //    var rot = transform.rotation * (currentRot * Quaternion.Inverse(prevRot));
        //    Debug.Log("interpolated by " + (currentPos - prevPos).ToString());

        //    rb.MovePosition(pos);
        //    rb.MoveRotation(rot);
        //}
    }

    private void Strategy4()
    {
        transform.position = currentPos;
        transform.rotation = currentRot;
    }

    private void Strategy3()
    {
        //lastPosD = transform.position;
        //lastR = transform.rotation;
        //while (snapshotQueue.Count > jitterBufferLenght + 1)
        //{
        //    snapshotQueue.TryDequeue(out var a);
        //    seqProcessed++;
        //}
        if (snapshotQueue.TryDequeue(out var playerState))
        {
            //if(seqProcessed> state.seqNo)
            //    return;
            var state = playerState.playerCoordinates;
            currentPos = new Vector3(state.posX, state.posY, state.posZ);
            currentRot = new Quaternion(state.rotX, state.rotY, state.rotZ, state.rotW);
            transform.position = currentPos;
            transform.rotation = currentRot;
            //seqProcessed++;
        }
        //else
        //{
        //    ExtrapolateTransform();
        //    seqProcessed++;
        //}
        //lastPosDelta = transform.position - lastPosD;
        //lastRotDelta = transform.rotation * Quaternion.Inverse(lastR);

        lastPosD = transform.position;
        lastR = transform.rotation;
    }

    float t1 = 0.01f;
    float t2 = 0.01f;
    internal Guid Id;
    internal string PlayerName;

    private void Strategy2()
    {
        t1 += Time.deltaTime;
        t2 += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, currentPos, t1);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentRot, t2);
        while (snapshotQueue.Count > jitterBufferLenght + 1)
        {
            snapshotQueue.TryDequeue(out var a);
            //currentPos = new Vector3(a.posX, a.posY, a.posZ);
            //currentRot = new Quaternion(a.rotX, a.rotY, a.rotZ, a.rotW);
            //t1 = 0.01f;
            //t2 = 0.01f;
        }
        if (snapshotQueue.TryDequeue(out var playerState))
        {
            var state = playerState.playerCoordinates;
            currentPos = new Vector3(state.posX, state.posY, state.posZ);
            currentRot = new Quaternion(state.rotX, state.rotY, state.rotZ, state.rotW);
            t1 = 0.5f;
            t2 = 0.5f;
        }
    }

    private void CosnumeSnapshot(PlayerState playerState)
    {
        var state = playerState.playerCoordinates;
        prevPos = currentPos;
        prevRot = currentRot;
        currentPos = new Vector3(state.posX, state.posY, state.posZ);
        currentRot = new Quaternion(state.rotX, state.rotY, state.rotZ, state.rotW);

        rb.MovePosition(currentPos);
        rb.MoveRotation(currentRot);
        //Debug.Log(" moved by " + (currentPos - prevPos).ToString());

       

    }

    private void Extrapolate()
    {
        //var pos = currentPos + (currentPos - prevPos);
        //var rot = currentRot * (currentRot * Quaternion.Inverse(prevRot));
        //prevPos = currentPos;
        //prevRot = currentRot;
        //currentPos = pos;
        //currentRot = rot;
        Debug.Log(" extrapolated by " + (lastPosDelta).ToString());

        rb.MovePosition(rb.position + lastPosDelta);
        rb.MoveRotation(rb.rotation * lastRotDelta);
        //rb.MovePosition(currentPos);
        //rb.MoveRotation(currentRot);
        jitterBufferLenght = Math.Min(maxJitterLenght, jitterBufferLenght + 1);
    }
    private void ExtrapolateTransform()
    {
        //var pos = currentPos + (currentPos - prevPos);
        //var rot = currentRot * (currentRot * Quaternion.Inverse(prevRot));
        //prevPos = currentPos;
        //prevRot = currentRot;
        //currentPos = pos;
        //currentRot = rot;
        Debug.Log(" extrapolated by " + (lastPosDelta).ToString());

        transform.position += lastPosDelta;
        transform.rotation *= lastRotDelta;
        jitterBufferLenght = Math.Min(maxJitterLenght, jitterBufferLenght + 1);
    }

    public void UpdatePosition(ulong sqeNo, PlayerState playerState)
    {
        //var state = playerState.playerCoordinates;
        //currentPos = new Vector3(state.posX, state.posY, state.posZ);
        //currentRot = new Quaternion(state.rotX, state.rotY, state.rotZ, state.rotW);
        //return;
       

        stateBuffer.TryAdd(sqeNo, playerState);
        if (stateBuffer.Count > jitterBufferLenght)
        {
            var valueColl = stateBuffer.Values;
            foreach (var item in valueColl)
            {
                snapshotQueue.Enqueue(item);
            }
            stateBuffer.Clear();
            if (seqProcessed == 0)
                seqProcessed = sqeNo;
            begin = true;

        }
       
        //this.transform.position = currentPos;
        //this.transform.rotation = currentRot;
        //rb.MovePosition(currentPos);
        //rb.MoveRotation(currentRot);
    }

}
