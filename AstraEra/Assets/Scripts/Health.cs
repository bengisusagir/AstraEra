using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public PhotonView pv;
    public Slider _slider;
    public Slider hbg;
    public bool isLocalPlayer;

    [Header("UI")]
    public TextMeshProUGUI healthText;
    public AudioSource died;
    private GameManager respawn;

    private bool isDead = false;

    public static Health instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        health = maxHealth;
        healthText.text = health.ToString();
        _slider.value = health;
        hbg.value = health;
    }

    [PunRPC]
    public void TakeDamage(int _damage)
    {
        // Prevent processing damage on an already dead player
        if (isDead)
            return;

        health -= _damage;
        health = Mathf.Max(health, 0);

        healthText.text = health.ToString();
        hbg.value = health;

        if (health <= 0)
        {
            isDead = true;
            died.Play();
            pv.RPC("PlayDieForAll", RpcTarget.Others);

            if (isLocalPlayer)
            {
                // Send all RPCs while the PhotonView is still valid
                pv.RPC("UpdateHealth", RpcTarget.OthersBuffered, 0);

                // Spawn the new player before destroying the old one
                if (GameManager.instance != null)
                    GameManager.instance.SpawnPlayer();

                PhotonNetwork.Destroy(gameObject);
            }
            return;
        }

        pv.RPC("UpdateHealth", RpcTarget.OthersBuffered, health);
    }

    [PunRPC]
    public void PlayDieForAll()
    {
        died.Play();
    }

    [PunRPC]
    private void UpdateHealth(int newHealth)
    {
        health = newHealth;
        _slider.value = newHealth;
        hbg.value = newHealth;
        healthText.text = newHealth.ToString();
    }
}
