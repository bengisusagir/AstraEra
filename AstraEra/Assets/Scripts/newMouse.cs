using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newMouse : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    [SerializeField]
    float mouseSensitivity;

    float xAxisClamp;

    [SerializeField]
    Transform player, playerArms;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (!PV.IsMine)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        RotateCamera();
    }

    [PunRPC]
    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;

        Vector3 rotPlayerArms = playerArms.transform.rotation.eulerAngles;
        Vector3 rotPlayer = player.transform.rotation.eulerAngles;

        rotPlayerArms.x -= rotAmountY;
        rotPlayerArms.z = 0;
        rotPlayer.y += rotAmountX;

        if (xAxisClamp > 90)
        {
            xAxisClamp = 90;
            rotPlayerArms.x = 90;
        }
        else if(xAxisClamp <-90)
        {
            xAxisClamp = -90;
            rotPlayerArms.x = 270;

        }

        playerArms.rotation = Quaternion.Euler(rotPlayerArms);
        player.rotation = Quaternion.Euler(rotPlayer);
    }


}