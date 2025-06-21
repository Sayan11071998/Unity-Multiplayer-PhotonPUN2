using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float health = 100f;
    public TextMeshProUGUI healthText;

    public GameManager gameManager;

    public void Hit(float damage)
    {
        health -= damage;
        healthText.text = "Health: " + health.ToString("F0");

        if (health <= 0f)
        {
            gameManager.EndGame();
        }
    }
}