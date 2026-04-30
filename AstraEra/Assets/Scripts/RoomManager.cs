using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject firstSc;
    public GameObject secSc;
    public GameObject thirdSc;
    public GameObject options;

    [Header("First Screen")]
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;
    [Header("Second Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;
    [Header("Third Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    public static RoomManager instance;

    [Header("Options")]
    public TextMeshProUGUI volumeAmount; 

    public string nickname = "unnamed";

    void Start()
    {
        instance = this;
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
        Debug.Log("Connecting");
    }

    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
        Debug.Log("Connected");
    }
    void SetScreen(GameObject screen)
    {
        firstSc.SetActive(false);
        secSc.SetActive(false);
        thirdSc.SetActive(false);
        options.SetActive(false);
        screen.SetActive(true);
    }
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }
    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public void SetAudio(float value)
    {
        AudioListener.volume = value;
        volumeAmount.text = ((int)(value*100)).ToString();
    }
    public void OptionsButton()
    {
        SetScreen(options);
    }

    public void GoBackMn()
    {
        SetScreen(firstSc);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
    public override void OnJoinedRoom()
    {
        SetScreen(thirdSc);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }
    public void OnStartButton()
    {
        
        SetScreen(secSc);

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(secSc);
    }
    public void OnStartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");

    }
    void Update()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Reconnect();
        }

    }
}
