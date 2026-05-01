using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
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
        // Use a generous upper bound to prevent index-out-of-range
        int maxPlayers = (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.MaxPlayers > 0)
            ? PhotonNetwork.CurrentRoom.MaxPlayers
            : 10;
        players = new Movement[maxPlayers];
        InvokeRepeating("spGuns", 2f, 15f);
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

        int prefabIndex = Random.Range(0, playerPrefabLocation.Length);
        int spawnIndex = Random.Range(0, spawnPoints.Length);

        GameObject playerObj = PhotonNetwork.Instantiate(
            playerPrefabLocation[prefabIndex],
            spawnPoints[spawnIndex].position,
            Quaternion.identity
        );

        PlayerSetup setup = playerObj.GetComponent<PlayerSetup>();
        if (setup != null)
        {
            setup.IsLocalPlayer();
        }

        playerObj.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
        playerObj.GetComponent<Health>().isLocalPlayer = true;
        PhotonNetwork.LocalPlayer.NickName = PlayerSetup.instance != null ? PlayerSetup.instance.nickname : "unnamed";
    }

    [PunRPC]
    public void GameOver()
    {
        if (NetworkManager.instance != null)
        {
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Menu");
            Destroy(NetworkManager.instance.gameObject);
        }
    }

    public Movement GetPlayer(int playerId)
    {
        return players.FirstOrDefault(x => x != null && x.id == playerId);
    }

    public Movement GetPlayer(GameObject playerObj)
    {
        return players.FirstOrDefault(x => x != null && x.gameObject == playerObj);
    }

    public void OnGoBackButton()
    {
        if (NetworkManager.instance != null)
        {
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Menu");
            Destroy(NetworkManager.instance.gameObject);
        }
    }

    public void spGuns()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.Instantiate(
            gunsLoc[Random.Range(0, gunsLoc.Length)],
            new Vector3(Random.Range(-35, 35), 6, Random.Range(-35, 35)),
            Quaternion.identity
        );

        PhotonNetwork.Instantiate(
            "ammo1",
            new Vector3(Random.Range(-35, 35), 1, Random.Range(-35, 35)),
            Quaternion.identity
        );
    }
}
