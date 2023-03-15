using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class DamageUIText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private Vector3 originalPos;
    private float dimLevel;
    private string text;
    private Color originalColor;
    private ConcurrentQueue<Action> todo =  new ConcurrentQueue<Action>();

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        originalPos=this.transform.position;
        originalColor = tmp.faceColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(todo.TryDequeue( out var work))
        {
            work.Invoke();
        }
       
        Do();
        
    }


    private void Do()
    {
        if (dimLevel > 0.1)
        {
            tmp.text = text;
            dimLevel -= Time.deltaTime;
        }
        else
        {
            dimLevel = 0;
            tmp.text = "";
        }
        int a = (int)(255 * dimLevel);
        tmp.faceColor = new Color32(tmp.faceColor.r, tmp.faceColor.g, tmp.faceColor.b, (byte)a);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y+1, originalPos.z);
    }

    public void UpdateText(string text)
    {
        todo.Enqueue(() => 
        {
            dimLevel = 1;
            this.text = text;
            this.transform.position = Input.mousePosition;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 80, originalPos.z);

            tmp.faceColor = originalColor;
        });
    }

}
