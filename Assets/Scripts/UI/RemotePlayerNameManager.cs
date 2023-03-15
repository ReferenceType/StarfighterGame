using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerNameManager : MonoBehaviour
{
    public GameObject RemotePlayerNamerReference;
    public static GameObject RemotePlayerNamerReference_;
    private static Dictionary<GameObject,GameObject> UIObjects= new Dictionary<GameObject, GameObject>();
    public static void AddPlayer(GameObject player, string playerName)
    {
        var UIObject = GameObject.Instantiate(RemotePlayerNamerReference_, RemotePlayerNamerReference_.transform.parent);
        var name =  UIObject.GetComponent<RemotePlayerNamer>();
        name.RemotePlayerName = playerName;
        name.RemotePlayer = player;
        UIObjects.Add(player, UIObject);
    }
    public static void RemovePlayer(GameObject player)
    {
        if (UIObjects.Remove(player, out var UIObj))
        {
            Destroy(UIObj);
        }
    }

    private void Awake()
    {
        RemotePlayerNamerReference_ = RemotePlayerNamerReference;
    }
}
