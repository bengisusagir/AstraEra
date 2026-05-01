using Photon.Pun;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public GameObject player;
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

        switch (gun)
        {
            case "spidergun":
                spidergun.SetActive(true);
                break;
            case "bluegun":
                bluegun.SetActive(true);
                break;
            case "crossbow":
                crossbow.SetActive(true);
                break;
            case "electricgun":
                electricgun.SetActive(true);
                break;
        }
    }

    [PunRPC]
    void TakeGun(string gun)
    {
        GameObject gobje = GameObject.Find(gun);
        if (gobje == null)
            return;

        taken.Play();
        gobje.SetActive(false);

        string gunName = gobje.name;

        if (gunName.StartsWith("spiderg"))
        {
            PV.RPC("SetGun", RpcTarget.All, "spidergun");
        }
        else if (gunName.StartsWith("blueg"))
        {
            PV.RPC("SetGun", RpcTarget.All, "bluegun");
        }
        else if (gunName.StartsWith("electricg"))
        {
            PV.RPC("SetGun", RpcTarget.All, "electricgun");
        }
        else if (gunName.StartsWith("crossbow"))
        {
            PV.RPC("SetGun", RpcTarget.All, "crossbow");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("gun"))
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
