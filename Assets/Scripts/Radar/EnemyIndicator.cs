using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    public float maxDistYellow = 750;
    public float maxDistRed= 500;
    private Material mat;
    private Color original;
    private bool isWhite = false;
    private bool isRed = false;
    private Transform camTransform;
    // Start is called before the first frame update
    void Start()
    {
        camTransform = Camera.main.transform;
        mat = GetComponent<MeshRenderer>().material;
        original = new Color(219 / 255f, 90 / 255f, 90 / 255f, 1f);
        isWhite = false;
        TurnRed();

    }

    // Update is called once per frame
    void Update()
    {
        var dist = this.transform.InverseTransformPoint(camTransform.position).magnitude;
        
        if ( dist < maxDistYellow && dist> maxDistRed)
        {
            TurnRed();
        }
        else if (dist < maxDistRed )
        {
            TurnRed();
        }
        else
            TurnWhite();

      
    }

    private void TurnYellow()
    {
        mat.color = Color.yellow;
    }

    public void TurnRed()
    {

        mat.color = original;

    }

    public void TurnWhite()
    {

        mat.color = Color.gray;
    }
}
