using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviourPun
{
    public int damage;
    public float fireRate;

    public new Camera camera;
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
    public Image ammoCircle;

    [Header("Animation")]
    public Animation anim;
    public AnimationClip reload;

    private bool isReloading = false;

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
        if (isReloading || mag <= 0)
            return;

        isReloading = true;
        anim.Play(reload.name);

        mag--;
        ammo = magAmmo;

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        SetAmmo();

        // Unlock reload after animation finishes
        StartCoroutine(ReloadCooldown(reload.length));
    }

    private IEnumerator ReloadCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isReloading = false;
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
        if (other.CompareTag("ammo"))
        {
            pv.RPC("TakeAmmo", RpcTarget.AllBuffered, other.gameObject.name);
        }
    }

    void Update()
    {
        if (!pv.IsMine)
            return;

        if (nextFire > 0)
            nextFire -= Time.deltaTime;

        if (nextFire <= 0 && ammo >= 0 && !anim.isPlaying && !isReloading)
        {
            // Block firing while ESC menu is open (sometimes it's stucks here)
            if (MouseLook.instance != null && MouseLook.instance.isESC)
                return;

            if (ammo == 0)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    PlayEmptyGunSound();
                }
            }
            else if (Input.GetButton("Fire1"))
            {
                nextFire = 1f / fireRate;
                ammo--;
                magText.text = mag.ToString();
                ammoText.text = ammo + "/" + magAmmo;
                SetAmmo();
                Fire();
                au.Play();
                pv.RPC("PlaySoundsForAll", RpcTarget.Others);
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

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            // Spawn VFX locally and on all other clients via RPC
            SpawnHitVFX(hit.point);
            pv.RPC("RpcSpawnHitVFX", RpcTarget.Others, hit.point);

            Health targetHealth = hit.transform.gameObject.GetComponent<Health>();
            if (targetHealth != null)
            {
                if (damage >= targetHealth.health)
                {
                    PhotonNetwork.LocalPlayer.AddScore(1);
                }

                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }

    [PunRPC]
    void RpcSpawnHitVFX(Vector3 position)
    {
        SpawnHitVFX(position);
    }

    void SpawnHitVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(hitVFX, position, Quaternion.identity);
        Destroy(vfx, 2f); // Auto-destroy after 2 seconds
    }
}
