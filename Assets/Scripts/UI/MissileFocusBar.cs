using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileFocusBar : MonoBehaviour
{
    public static float Progress01;
    public Image FillImg;
    public Image FillImgBG;

  
    void Update()
    {
        if (Progress01 > 0)
        {
            FillImgBG.enabled = true;
            FillImg.enabled = true;
            FillImg.fillAmount= Mathf.Clamp01(Progress01);
            this.transform.position = Input.mousePosition;
        }
        else
        {
            FillImgBG.enabled=false;
            FillImg.enabled=false;
        }
    }
}
