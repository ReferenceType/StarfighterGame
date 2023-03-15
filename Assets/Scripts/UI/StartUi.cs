using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUi : MonoBehaviour
{
    
    public InputField inpitfield;
    public Button StartBtn;
    public Slider slider;
    public static string playerName;
    private void Start()
    {
        StartBtn.onClick.AddListener(StartClicked);
       
    }

    private void StartClicked()
    {
        playerName = inpitfield.text;
        SceneManager.LoadScene("GameScene");
        AudioListener.volume = slider.value;
    }
}
