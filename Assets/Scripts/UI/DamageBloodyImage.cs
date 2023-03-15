using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageBloodyImage : MonoBehaviour
{
    static bool draw;
    Image img;
    void Start()
    {
        img = GetComponent<Image>();
    }

    static int drawCnt = 0;
    void Update()
    {
        if (draw)
        {
            drawCnt--;
            img.enabled = true;
        }
        if(drawCnt ==0 )
        {
            draw = false;
            drawCnt--;
            img.enabled = false;
        }
    }

    public static void Draw()
    {
        drawCnt= 9;
        draw= true;
    }
}
