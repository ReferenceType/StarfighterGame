using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Radar : MonoBehaviour
{
    public RadarIndicator Indicator;
    public GameObject Enemy;
    private Camera mainCam;
    public float Wideness = 30f;
    public TextMeshPro text;
    public Dictionary<GameObject, RadarIndicator> Enemies = new Dictionary<GameObject, RadarIndicator>();
    private Vector3 initPos;
    
    void Start()
    {
        mainCam = Camera.main;
        initPos = text.gameObject.transform.localPosition;
        if (Enemy != null)
            AddEnemy(Enemy);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var e in Enemies)
        {
            var enemy = e.Key;
            var indicator = e.Value;

            var TargetProjected = CalculateRelativeProjection(enemy, indicator, out float zDistance);
            indicator.Indicator.transform.localPosition = TargetProjected / (Wideness * zDistance);
            if (indicator.Indicator.transform.localPosition.magnitude > 0.5)
            {
                indicator.Indicator.transform.localPosition = indicator.Indicator.transform.localPosition.normalized / 2;

            }
            indicator.DistanceText.gameObject.transform.localPosition = indicator.Indicator.transform.localPosition + initPos;
        }
        

    }
    Vector3 CalculateRelativeProjection(GameObject enemy, RadarIndicator indicator, out float distance)
    {
        Vector3 TargetProjected;

        var relative = this.mainCam.gameObject.transform.InverseTransformPoint(enemy.transform.position);
        float radarScaleFactor = Mathf.Clamp(1/(3*Mathf.Sqrt(relative.magnitude)), 0.01f, 0.025f);

        indicator.Indicator.transform.localScale = new Vector3(radarScaleFactor, radarScaleFactor, 0.002f);
        indicator.DistanceText.text = relative.magnitude.ToString("N0")+"m";

        distance = Mathf.Abs(relative.z);
        if (relative.z > 0)
            TargetProjected = new Vector3(relative.x, relative.y, 0);
        else
            TargetProjected = new Vector3(relative.x, relative.y,0 )*500;

        return TargetProjected;
    }

    public void AddEnemy(GameObject enemy)
    {
        var enemyIndicator = Instantiate(Indicator);
        enemyIndicator.transform.parent = Indicator.transform.parent;
        enemyIndicator.transform.localPosition = Vector3.zero;
        enemyIndicator.transform.localRotation = Quaternion.identity;
        enemyIndicator.gameObject.SetActive(true);

        Enemies.Add(enemy, enemyIndicator);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if(Enemies.Remove(enemy, out var enemyIndicator))
        {
            Destroy(enemyIndicator.gameObject);
        }
    }
}
