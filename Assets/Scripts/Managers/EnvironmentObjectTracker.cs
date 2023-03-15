using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObjectTracker : MonoBehaviour
{
    public static Dictionary<string, GameObject> sceneObjectsById = new Dictionary<string, GameObject>();
    public static Dictionary<GameObject, string> sceneObjectsByObject = new Dictionary<GameObject, string>();

    internal static void AddObject(GameObject spaceship)
    {
        Transform[] objectsWithCollider = spaceship.GetComponentsInChildren<Transform>();
        foreach (var c in objectsWithCollider)
        {
            string uniqueName = "";
            var tr = c.gameObject.transform;
            while (tr != null)
            {
                uniqueName += tr.name;
                uniqueName += tr.GetSiblingIndex().ToString();
                tr = tr.parent;
            }
            sceneObjectsById.Add(uniqueName, c.gameObject);
            sceneObjectsByObject.Add(c.gameObject, uniqueName);
        }
    }

    void Start()
    {
        Transform[] objectsWithCollider = GetComponentsInChildren<Transform>();
        foreach (var c in objectsWithCollider)
        {
            string uniqueName = "";
            var tr = c.gameObject.transform;
            while (tr != null)
            {
                uniqueName += tr.name;
                uniqueName += tr.GetSiblingIndex().ToString();
                tr = tr.parent;
            }
            sceneObjectsById.Add(uniqueName, c.gameObject);
        }

        foreach (var item in sceneObjectsById)
        {
            sceneObjectsByObject.Add(item.Value, item.Key);
        }
    }

}
