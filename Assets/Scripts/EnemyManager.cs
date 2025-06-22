using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public Animator enemyAnimator;
    public float damage = 20f;
    public float health = 100f;
    public GameManager gameManager;

    public Slider slider;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        slider.maxValue = health;
        slider.value = health;
    }

    private void Update()
    {
        slider.transform.LookAt(player.transform);

        navMeshAgent.destination = player.transform.position;

        if (navMeshAgent.velocity.magnitude > 1f)
        {
            enemyAnimator.SetBool("isRunning", true);
        }
        else
        {
            enemyAnimator.SetBool("isRunning", false);
        }
    }

    public void Hit(float damage)
    {
        health -= damage;
        slider.value = health;

        if (health <= 0)
        {
            gameManager.enemiesAlive--;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == player)
        {
            player.GetComponent<PlayerManager>().Hit(damage);
        }
    }
}