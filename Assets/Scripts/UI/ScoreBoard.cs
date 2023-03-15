using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public ScoreEntry TemplateReference;
    public static ScoreEntry Template;
    private static ConcurrentDictionary<Guid, ScoreEntry> scores = new ConcurrentDictionary<Guid, ScoreEntry>();

    private void Awake()
    {
        if (Template == null)
            Template = TemplateReference;
       
    }
   
    public void Test(float val)
    {
        var pl = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        pl.renderScale= val;
    }

    private void Update()
    {
       if(Input.GetKey(KeyCode.Tab))
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
       else
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

    }
    public static void AddPlayer(string playerName, Guid playerId, bool highlight = false)
    {
        var entry = Instantiate(Template, Template.transform.parent);
        entry.transform.position = Template.transform.position;
        entry.Initialise(playerName);
        scores.TryAdd(playerId, entry);
        entry.SetIndex(scores.Count);
        if(highlight) entry.Highlight();
        CalculateTableOrder();
    }

    public static void IncrementKills(Guid playerId)
    {
        if (scores.TryGetValue(playerId, out var entry))
        {
            entry.IncrementKills();
            CalculateTableOrder();
        }
    }

    public static void IncrementDeaths(Guid playerId)
    {
        if (scores.TryGetValue(playerId, out var entry))
        {
            entry.IncrementDeaths();
            CalculateTableOrder();
        }
    }

    private static void CalculateTableOrder()
    {
        var ordered = scores.OrderByDescending(x => x.Value.Score).ToDictionary(x => x.Key, x => x.Value);
        int i = 0;
        foreach (var item in ordered)
        {
            item.Value.SetIndex(i);
            i++;
        }
    }
}
