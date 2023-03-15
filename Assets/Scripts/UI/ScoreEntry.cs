using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry : MonoBehaviour
{
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI Kills;
    public TextMeshProUGUI Deaths;
    public TextMeshProUGUI ScoreTxt;
    public string playerName;
    public int kills = 0;
    public int deaths = 0;
    public int index = 0;
    public int margin = 50;
    public int zeromargin = -193;
    public int Score => kills-deaths;

    public void Initialise(string playerName)
    {
        this.playerName = playerName;
        PlayerName.text= playerName;
        Kills.text = "0";
        Deaths.text = "0";
        ScoreTxt.text = Score.ToString();
        index = 0;
    }

    public void IncrementKills()
    {
        kills++;
        Kills.text = kills.ToString();
        ScoreTxt.text = Score.ToString();

    }
    public void IncrementDeaths()
    {
        deaths++;
        Deaths.text = deaths.ToString();
        ScoreTxt.text = Score.ToString();

    }

    public void Highlight()
    {
        //100 67 67 100
        GetComponentInChildren<Image>().color = new Color(100f/255, 67f/255, 67f/255, 100f/255);
        GetComponentInChildren<Image>().color = new Color(100f/255, 67f/255, 67f/255, 100f/255);
    }

    public void SetIndex(int idx)
    {
        this.transform.SetSiblingIndex(idx+1);
        //var pos = GetComponent<RectTransform>();
        //    pos.position = new Vector3(pos.position.x, yPos - ((idx+1) * (margin)), 0);
    }
}
