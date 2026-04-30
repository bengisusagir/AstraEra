using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviourPun
{
    public int damage;
    public float fireRate;

    public Camera camera;
    public GameObject spi;
    private float nextFire;
    public AudioSource au;
    public AudioSource reloads;
    public AudioSource taken;

    public PhotonView pv;


    [Header("VFX")]
    public GameObject hitVFX;

    [Header("Ammo")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magText;
    private bool isGunEmpty = false;
    public Image ammoCircle;

    [Header("Animation")]
    public Animation anim;
    public AnimationClip reload;


    // Update is called once per frame
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        SetAmmo();

    }
    void SetAmmo()
    {
        ammoCircle.fillAmount = (float)ammo / magAmmo;
    }
    void Reload()
    {
        anim.Play(reload.name);
        if (mag > 0)
        {
            mag--;
            ammo = magAmmo;
        }

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        SetAmmo();

    }
    [PunRPC]
    void TakeAmmo(string ammos)
    {

        GameObject gobje = GameObject.Find(ammos);
        if (gobje != null)
        {
            taken.Play();
        gobje.SetActive(false);
            mag++;
            SetAmmo();
            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ammo")
        {
            pv.RPC("TakeAmmo", RpcTarget.AllBuffered,other.gameObject.name);

        }

    }

    void Update()
    {
        if (!pv.IsMine)
            return;


        if (nextFire > 0)
            nextFire -= Time.deltaTime;
        if ( nextFire <= 0 && ammo >= 0 && anim.isPlaying==false)
        {
            if (ammo == 0)
            {
                isGunEmpty = true;
                if (Input.GetButtonDown("Fire1"))
                {
                    PlayEmptyGunSound();
                }
            }
            else if(Input.GetButton("Fire1"))
            {
            if (MouseLook.instance.isESC)
            {
                MouseLook.instance.isESC = true;
            }
            nextFire = 1 / fireRate;
            ammo--;
            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;
            SetAmmo();
            Fire();
            au.Play();
            pv.RPC("PlaySoundsForAll", RpcTarget.All);
            }
            else if (ammo > 0)
            {
                isGunEmpty = false; 
            }

        }
        
        if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
    }

    void PlayEmptyGunSound()
    {
        reloads.PlayOneShot(reloads.clip);
    }
    [PunRPC]
    public void PlaySoundsForAll()
    {
        au.Play();

    }
    void Fire()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray.origin,ray.direction,out hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);
            

            if (hit.transform.gameObject.GetComponent<Health>())
            {
                if(damage >= hit.transform.gameObject.GetComponent<Health>().health)
                {
                    PhotonNetwork.LocalPlayer.AddScore(1);
                }

                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All,damage);
            }
        }
    }
}
