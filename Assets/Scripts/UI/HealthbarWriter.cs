using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarWriter : MonoBehaviour
{
    public TextMeshProUGUI BarText;
    public Image Bar;
  
    void Update()
    {
        BarText.text = ((int)(Bar.fillAmount * 100)).ToString();
    }
}
