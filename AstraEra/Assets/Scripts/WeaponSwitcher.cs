using Nokobot.Assets.Crossbow;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public GameObject player;
    private int selectedWeapon = 0;
    public GameObject spidergun;
    public GameObject bluegun;
    public GameObject crossbow;
    public GameObject electricgun;
    public AudioSource taken;
    void Start()
    {
        PV = player.GetComponent<PhotonView>();
        
    }
    [PunRPC]
    void SetGun(string gun)
    {
        spidergun.SetActive(false);
        bluegun.SetActive(false);
        crossbow.SetActive(false);
        electricgun.SetActive(false);

        if(gun=="spidergun")
            spidergun.SetActive(true);
        if (gun == "bluegun")
            bluegun.SetActive(true);
        if (gun == "crossbow")
            crossbow.SetActive(true);
        if (gun == "electricgun")
            electricgun.SetActive(true);

    }
    [PunRPC]
    void TakeGun(string gun)
    {

        GameObject gobje = GameObject.Find(gun);
        if (gobje != null)
        {
            taken.Play();
            gobje.SetActive(false);
            if(gobje.name=="spiderg(Clone)")
            {
            spidergun.SetActive(true);
            PV.RPC("SetGun", RpcTarget.All, "spidergun"); 
            }
            else if (gobje.name == "blueg(Clone)")
            {
                bluegun.SetActive(true);
                PV.RPC("SetGun", RpcTarget.All, "bluegun");
            }
            else if (gobje.name == "electricg(Clone)")
            {
                electricgun.SetActive(true);
                PV.RPC("SetGun", RpcTarget.All, "electricgun");
            }
            else if (gobje.name == "crossbow(Clone)")
            {
                crossbow.SetActive(true);
                PV.RPC("SetGun", RpcTarget.All, "crossbow");
            }

        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "gun")
        {
            PV.RPC("TakeGun", RpcTarget.AllBuffered, other.gameObject.name);

        }

    }
    void Update()
    {
        if (!PV.IsMine)
            return;


    }
}
