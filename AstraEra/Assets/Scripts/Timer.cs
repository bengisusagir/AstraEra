using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public float limitedTime;
    public PhotonView pv;

    private bool matchEnded = false;

    private void Update()
    {
        if (matchEnded)
            return;

        if (limitedTime > 0)
        {
            limitedTime -= Time.deltaTime;
        }

        if (limitedTime <= 0)
        {
            limitedTime = 0;
            matchEnded = true;

            pv.RPC("Winner", RpcTarget.All);
            Invoke("CallGameover", 3f);

            if (MouseLook.instance != null)
                MouseLook.instance.isESC = true;
        }

        int minutes = Mathf.FloorToInt(limitedTime / 60);
        int seconds = Mathf.FloorToInt(limitedTime % 60);
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void CallGameover()
    {
        pv.RPC("GameOver", RpcTarget.All);
    }
}
