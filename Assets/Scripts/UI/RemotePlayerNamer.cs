using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemotePlayerNamer : MonoBehaviour
{
    public GameObject RemotePlayer;
    public string RemotePlayerName;
    private TextMeshProUGUI txt;
    private Transform mainCamTransform;
    private Camera mainCam;
    private Transform remotePlayerTransform;

    void Start()
    {
        txt=GetComponent<TextMeshProUGUI>();
        txt.text = RemotePlayerName;
        mainCamTransform = Camera.main.transform;
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (RemotePlayer == null)
            return;
        if(remotePlayerTransform == null)
            remotePlayerTransform=RemotePlayer.transform;

        if (mainCamTransform.InverseTransformPoint(remotePlayerTransform.position).z > 0)
        {
            txt.text = RemotePlayerName;
            var vec = new Vector3(remotePlayerTransform.position.x, remotePlayerTransform.position.y, remotePlayerTransform.position.z);
            vec += 10* mainCam.transform.up;
            this.transform.position = mainCam.WorldToScreenPoint(vec);
        }
        else
        {
            txt.text = "";
        }
    }

    

}
