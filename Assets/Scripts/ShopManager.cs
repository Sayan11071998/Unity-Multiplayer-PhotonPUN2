using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Text priceNumber;
    [SerializeField] private Text priceText;

    [SerializeField] private int price = 50;

    [SerializeField] private bool HealthStation;
    [SerializeField] private bool ammoStation;

    private PlayerManager playerManager;
    private bool playerIsInReach = false;

    private void Start() => priceNumber.text = price.ToString();

    private void Update()
    {
        if (playerIsInReach)
        {
            if (Input.GetKeyDown(KeyCode.E))
                BuyShop();
        }
    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            priceNumber.gameObject.SetActive(true);
            priceText.gameObject.SetActive(true);
            playerIsInReach = true;
            playerManager = player.gameObject.GetComponent<PlayerManager>();
        }
    }

    private void OnTriggerExit(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            priceNumber.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);
            playerIsInReach = false;
        }
    }

    public void BuyShop()
    {
        if (playerManager.currentPoints >= price)
        {
            playerManager.currentPoints -= price;

            if (HealthStation)
            {
                playerManager.health = playerManager.healthCap;
                playerManager.healthNumber.text = playerManager.health.ToString();
            }

            if (ammoStation)
            {
                foreach (Transform child in playerManager.weaponHolder.transform)
                {
                    if (child.gameObject.activeSelf)
                    {
                        WeaponManager weaponManager = child.gameObject.GetComponent<WeaponManager>();
                        weaponManager.reserveAmmo = weaponManager.ammoCap;
                        StartCoroutine(weaponManager.Reload(weaponManager.reloadTime));
                        weaponManager.reserveAmmoText.text = weaponManager.reserveAmmo.ToString();
                    }
                }
            }
        }
    }
}