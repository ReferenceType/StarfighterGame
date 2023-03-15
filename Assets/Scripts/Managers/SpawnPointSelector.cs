using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointSelector : MonoBehaviour
{
    private static Transform[] SpawnPoints;

    // Start is called before the first frame update
    void Awake()
    {
       SpawnPoints = GetComponentsInChildren<Transform>();
    }

    public static Transform GetSpawnPoint()
    {
        int idx = Random.Range(1, SpawnPoints.Length);
        return SpawnPoints[idx];
    }
    
}
