using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenUi : MonoBehaviour
{
    public InputField InputField;
    public Button StartBtn;
    public static string playerName;
    public int why;
    private void Start()
    {
        StartBtn.onClick.AddListener(StartClicked);
    }

    private void StartClicked()
    {
        playerName = InputField.text;
    }
}
