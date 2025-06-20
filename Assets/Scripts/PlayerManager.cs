using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public float health = 100f;

    public void Hit(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            SceneManager.LoadScene(0);
        }
    }
}