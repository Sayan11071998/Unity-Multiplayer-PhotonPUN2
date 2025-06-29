using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public GameManager gameManager;

    [SerializeField] private GameObject player;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Slider slider;
    [SerializeField] private AudioClip[] zombieSounds;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float damage = 20f;
    [SerializeField] private float health = 100;
    [SerializeField] private int points = 20;

    [SerializeField] private bool playerInReach;
    [SerializeField] private float attackDelayTimer;
    [SerializeField] private float howMuchEarlierStartAttackAnim;
    [SerializeField] private float delayBetweenAttacks;

    [SerializeField] private PhotonView photonView;

    private GameObject[] playersUInScene;
    private NavMeshAgent navMeshAgent;

    public float Health => health;
    public float Points => points;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playersUInScene = GameObject.FindGameObjectsWithTag("Player");

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

        GetClosestPlayer();

        if (player != null)
        {
            slider.gameObject.transform.LookAt(player.transform.position);
            navMeshAgent.destination = player.transform.position;
        }

        if (navMeshAgent.velocity.magnitude > 1)
            enemyAnimator.SetBool("isRunning", true);
        else
            enemyAnimator.SetBool("isRunning", false);
    }

    private void GetClosestPlayer()
    {
        float minDistance = Mathf.Infinity;
        Vector3 currPosition = transform.position;

        foreach (GameObject thisPlayer in playersUInScene)
        {
            if (thisPlayer != null)
            {
                float distance = Vector3.Distance(thisPlayer.transform.position, currPosition);

                if (distance < minDistance)
                {
                    player = thisPlayer;
                    minDistance = distance;
                }
            }
        }
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
        photonView.RPC("TakeDamage", RpcTarget.All, damage, photonView.ViewID);
    }

    [PunRPC]
    public void TakeDamage(float damage, int ViewID)
    {
        if (photonView.ViewID == ViewID)
        {
            health -= damage;
            slider.value = health;
            if (health <= 0)
            {
                enemyAnimator.SetTrigger("isDead");
                Destroy(gameObject, 10f);

                if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
                    gameManager.enemiesAlive--;

                slider.gameObject.SetActive(false);
                Destroy(navMeshAgent);
                Destroy(this);
                Destroy(GetComponent<CapsuleCollider>());
            }
        }
    }
}