using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    public GameObject playersHolder;
    public GameObject winnerpanel;

    public float refreshRate = 1f;
    public GameObject[] slots;
    [Space]
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI winnername;
    public TextMeshProUGUI winnerscore;

    public static ScoreBoard instance;

    private void Awake()
    {
        instance = this;
    }

    [PunRPC]
    public void Winner()
    {
        winnerpanel.SetActive(true);
        var winnerplayer = PhotonNetwork.PlayerList
            .OrderByDescending(p => p.GetScore())
            .FirstOrDefault();

        if (winnerplayer != null)
        {
            winnername.text = winnerplayer.NickName;
            winnerscore.text = winnerplayer.GetScore().ToString();
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    public void Refresh()
    {
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        var sortedPlayers = PhotonNetwork.PlayerList
            .OrderByDescending(p => p.GetScore());

        int i = 0;
        foreach (var player in sortedPlayers)
        {
            // Prevent overflow if more players than UI slots
            if (i >= slots.Length)
                break;

            slots[i].SetActive(true);

            if (string.IsNullOrEmpty(player.NickName))
                player.NickName = "unnamed";

            nameTexts[i].text = player.NickName;
            scoreTexts[i].text = player.GetScore().ToString();

            i++;
        }
    }

    private void Update()
    {
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}
