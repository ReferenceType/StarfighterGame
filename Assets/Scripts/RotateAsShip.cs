using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 // TODO Remove
public class RotateAsShip : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Player;
   
    void Update()
    {
        this.transform.localRotation= Player.transform.rotation;
    }
}
