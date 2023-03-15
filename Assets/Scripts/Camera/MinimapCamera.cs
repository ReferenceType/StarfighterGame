using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        this.transform.position = new Vector3(player.transform.position.x, this.transform.position.y,player.transform.position.z );
    }
}
