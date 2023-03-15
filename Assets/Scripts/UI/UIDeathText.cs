using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UIDeathText : MonoBehaviour
{
    static bool write = false;
    private static int cnt;
    private TextMeshProUGUI txt;
    static string text;

    // Start is called before the first frame update
    void Start()
    {
        txt=GetComponent<TextMeshProUGUI>();
    }

    
    void Update()
    {
        if (write)
        {
            txt.text=text;
            cnt--;
        }
        if(cnt == 0)
        {
            write = false;
            txt.text = "";
            cnt--;
        }

    }

    public static void WriteText(string tx)
    {
        text = tx;
        write = true;
        cnt = 240;
    }
}
