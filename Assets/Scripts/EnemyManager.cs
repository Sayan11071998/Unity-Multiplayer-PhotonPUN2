using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;
    public Animator enemyAnimator;
    public Slider slider;
    public AudioClip[] zombieSounds;
    public AudioSource audioSource;

    public float damage = 20f;
    public float health = 100;
    public int points = 20;

    public bool playerInReach;
    private float attackDelayTimer;
    public float howMuchEarlierStartAttackAnim;
    public float delayBetweenAttacks;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        slider.maxValue = health;
        slider.value = health;
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.Play();
        }

        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) return;

        slider.gameObject.transform.LookAt(player.transform.position);

        navMeshAgent.destination = player.transform.position;

        if (navMeshAgent.velocity.magnitude > 1)
            enemyAnimator.SetBool("isRunning", true);
        else
            enemyAnimator.SetBool("isRunning", false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
            playerInReach = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (playerInReach)
            attackDelayTimer += Time.deltaTime;

        if (attackDelayTimer >= delayBetweenAttacks - howMuchEarlierStartAttackAnim && attackDelayTimer <= delayBetweenAttacks && playerInReach)
            enemyAnimator.SetTrigger("isAttacking");

        if (attackDelayTimer >= delayBetweenAttacks && playerInReach)
        {
            player.GetComponent<PlayerManager>().Hit(damage);
            attackDelayTimer = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }

    public void Hit(float damage)
    {
        health -= damage;
        slider.value = health;
        if (health <= 0)
        {
            enemyAnimator.SetTrigger("isDead");
            Destroy(gameObject, 10f);
            gameManager.enemiesAlive--;
            slider.gameObject.SetActive(false);
            Destroy(navMeshAgent);
            Destroy(this);
            Destroy(GetComponent<CapsuleCollider>());
        }
    }
}