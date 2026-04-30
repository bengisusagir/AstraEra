using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Globalization;
using System.Security.Cryptography;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player")]
    public string[] playerPrefabLocation;
    public Transform[] spawnPoints;
    public string[] gunsLoc;
    public string[] ammo;

    public Movement[] players;
    private int playersInGame;
    public Timer timer;
    public GameObject roomCam;



    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        InvokeRepeating("spGuns", 2f, 15f);
        players = new Movement[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.All);
    }


    public void UpdateName(GameObject player)
    {
        player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);

    }

    [PunRPC]
    public void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
            timer.enabled = true;
        }
    }
    public void SpawnPlayer()
    {
        roomCam.SetActive(false);
        GameObject playerObj = (GameObject)PhotonNetwork.Instantiate(playerPrefabLocation[Random.Range(0, playerPrefabLocation.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        PlayerSetup.instance.IsLocalPlayer();
        playerObj.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
        playerObj.GetComponent<Health>().isLocalPlayer = true;
        PhotonNetwork.LocalPlayer.NickName = PlayerSetup.instance.nickname;
        

    }
    [PunRPC]
    public void GameOver()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Menu");
        Destroy(NetworkManager.instance.gameObject);


    }

    public Movement GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }
    public Movement GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }
    public void OnGoBackButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Menu");
        Destroy(NetworkManager.instance.gameObject);
    }
    public void spGuns()
    { 
            
           GameObject guns = (GameObject)PhotonNetwork.Instantiate(gunsLoc[Random.Range(0, gunsLoc.Length)],new Vector3(Random.Range(-35,35), 6, Random.Range(-35, 35)), Quaternion.identity);
           GameObject ammo = (GameObject)PhotonNetwork.Instantiate("ammo1",new Vector3(Random.Range(-35,35), 1, Random.Range(-35, 35)), Quaternion.identity);

    }
 
}
