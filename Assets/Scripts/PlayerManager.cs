using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float health = 100f;
    public TextMeshProUGUI healthText;

    public GameManager gameManager;
    public GameObject playerCamera;

    private Quaternion playerCameraOriginalRotation;

    private float shakeTime;
    private float shakeDuration;

    private void Start()
    {
        playerCameraOriginalRotation = playerCamera.transform.localRotation;
    }

    private void Update()
    {
        if (shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }
        else if (playerCamera.transform.localRotation != playerCameraOriginalRotation)
        {
            playerCamera.transform.localRotation = playerCameraOriginalRotation;
        }
    }

    public void Hit(float damage)
    {
        health -= damage;
        healthText.text = "Health: " + health.ToString("F0");

        if (health <= 0f)
        {
            gameManager.EndGame();
        }
        else
        {
            shakeTime = 0f;
            shakeDuration = 0.2f;
        }
    }

    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }
}