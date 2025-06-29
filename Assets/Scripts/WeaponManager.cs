using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject playerCam;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private GameObject nonTargetHitParticles;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip gunshot;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private WeaponSway weaponSway;
    [SerializeField] private Text currentAmmoText;

    public Text reserveAmmoText;

    public float currentAmmo;
    public float maxAmmo;
    public float reserveAmmo;
    public float reloadTime = 2f;
    public float firerate = 10;
    public float ammoCap;

    public int range = 100;
    public int damage = 25;

    [SerializeField] private bool isAutomatic;

    [SerializeField] private string weaponType;

    [SerializeField] private PhotonView photonView;

    private float swaySensitivity;
    private float firerateTimer = 0;

    private bool isReloading;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        swaySensitivity = weaponSway.swaySensitivity;

        currentAmmoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reserveAmmo.ToString();

        ammoCap = reserveAmmo;
    }

    private void Update()
    {
        if (photonView != null && !photonView.IsMine) return;

        if (playerAnimator.GetBool("isShooting"))
            playerAnimator.SetBool("isShooting", false);

        if (reserveAmmo <= 0 && currentAmmo <= 0) return;

        if (currentAmmo <= 0 && isReloading == false)
        {
            StartCoroutine(Reload(reloadTime));
            return;
        }

        if (isReloading == true) return;

        if (Input.GetKeyDown(KeyCode.R) && reserveAmmo > 0)
        {
            StartCoroutine(Reload(reloadTime));
            return;
        }

        if (firerateTimer > 0)
            firerateTimer = firerateTimer - Time.deltaTime;

        if (Input.GetButton("Fire1") && firerateTimer <= 0 && isAutomatic)
        {
            Shoot();
            firerateTimer = 1 / firerate;
        }

        if (Input.GetButtonDown("Fire1") && firerateTimer <= 0 && !isAutomatic)
        {
            Shoot();
            firerateTimer = 1 / firerate;
        }

        if (Input.GetButtonDown("Fire2"))
            Aim();

        if (Input.GetButtonUp("Fire2"))
        {
            if (playerAnimator.GetBool("isAiming"))
                playerAnimator.SetBool("isAiming", false);

            weaponSway.swaySensitivity = swaySensitivity;
            crosshair.SetActive(true);
        }
    }

    private void OnEnable()
    {
        playerAnimator.SetTrigger(weaponType);
        currentAmmoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reserveAmmo.ToString();
    }

    private void OnDisable()
    {
        playerAnimator.SetBool("isReloading", false);
        isReloading = false;
    }

    private void Shoot()
    {
        currentAmmo--;
        currentAmmoText.text = currentAmmo.ToString();

        if (PhotonNetwork.InRoom)
            photonView.RPC("WeaponShootVFX", RpcTarget.All, photonView.ViewID);
        else
            ShootVFX(photonView.ViewID);

        playerAnimator.SetBool("isShooting", true);

        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.Hit(damage);
                if (enemyManager.Health <= 0)
                    playerManager.currentPoints += enemyManager.Points;

                GameObject InstParticles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                InstParticles.transform.parent = hit.transform;
                Destroy(InstParticles, 2f);
            }
            else
            {
                GameObject InstParticles = Instantiate(nonTargetHitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(InstParticles, 20f);
            }
        }
    }

    public void ShootVFX(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            muzzleFlash.Play();
            audioSource.PlayOneShot(gunshot, 1f);
        }
    }

    private void Aim()
    {
        playerAnimator.SetBool("isAiming", true);
        weaponSway.swaySensitivity = swaySensitivity / 3;
        crosshair.SetActive(false);
    }

    public IEnumerator Reload(float rt)
    {
        isReloading = true;
        playerAnimator.SetBool("isReloading", true);

        yield return new WaitForSeconds(rt);

        playerAnimator.SetBool("isReloading", false);
        float missingAmmo = maxAmmo - currentAmmo;

        if (reserveAmmo >= missingAmmo)
        {
            currentAmmo += missingAmmo;
            reserveAmmo -= missingAmmo;
            currentAmmoText.text = currentAmmo.ToString();
            reserveAmmoText.text = reserveAmmo.ToString();
        }
        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
            currentAmmoText.text = currentAmmo.ToString();
            reserveAmmoText.text = reserveAmmo.ToString();
        }

        isReloading = false;
    }
}