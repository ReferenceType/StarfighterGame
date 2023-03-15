using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
  
    void Update()
    {
        Cursor.visible = false;

        this.transform.position = Input.mousePosition;
    }
}
