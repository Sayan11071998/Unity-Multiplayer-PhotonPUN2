using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public float health = 100f;
    public TextMeshProUGUI healthText;

    public void Hit(float damage)
    {
        health -= damage;
        healthText.text = "Health: " + health.ToString("F0");

        if (health <= 0f)
        {
            SceneManager.LoadScene(0);
        }
    }
}