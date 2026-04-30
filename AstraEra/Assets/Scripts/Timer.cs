using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public float limitedTime;
    public PhotonView pv;


    private void Update()
    {
        if (limitedTime > 0 )
        {
            limitedTime -= Time.deltaTime;
        }
        if (limitedTime < 0)
        {
            limitedTime = 0;
            pv.RPC("Winner", RpcTarget.All);
            Invoke("CallGameover", 3f);
            MouseLook.instance.isESC = true ;

        }
        int minutes1 = Mathf.FloorToInt(limitedTime / 60);
        int seconds1 = Mathf.FloorToInt(limitedTime % 60);
        timer.text = string.Format("{0:00}:{1:00}", minutes1, seconds1);
        
    }

    void CallGameover()
    {
        pv.RPC("GameOver", RpcTarget.All);
    }

}
