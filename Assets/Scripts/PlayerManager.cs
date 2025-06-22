using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float health = 100f;
    public TextMeshProUGUI healthText;

    public GameManager gameManager;
    public GameObject playerCamera;
    public GameObject hurtPanelGameObject;
    public CanvasGroup hurtPanel;

    private Quaternion playerCameraOriginalRotation;

    private float shakeTime;
    private float shakeDuration;

    private void Start()
    {
        playerCameraOriginalRotation = playerCamera.transform.localRotation;
        hurtPanel.alpha = 0f;
        hurtPanelGameObject.SetActive(true);
    }

    private void Update()
    {
        if (hurtPanel.alpha > 0f)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }

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
            hurtPanel.alpha = 0f;
            hurtPanelGameObject.SetActive(false);
        }
        else
        {
            shakeTime = 0f;
            shakeDuration = 0.2f;
            hurtPanel.alpha = 1f;
        }
    }

    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }
}