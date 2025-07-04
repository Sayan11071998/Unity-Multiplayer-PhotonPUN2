using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Text roundNumber;
    [SerializeField] private Text roundsSurvived;

    public int enemiesAlive = 0;
    [SerializeField] private int round = 0;

    [SerializeField] private new PhotonView photonView;

    private void Start() => spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");

    private void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if (enemiesAlive == 0)
            {
                round++;
                NextWave(round);

                if (PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("CurrentRound", round);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayNextRound(round);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeSelf)
                Pause();
            else
                Continue();
        }
    }

    private void DisplayNextRound(int round) => roundNumber.text = round.ToString();

    public void NextWave(int round)
    {
        for (var x = 0; x < round; x++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemySpawned;

            if (PhotonNetwork.InRoom)
                enemySpawned = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            else
                enemySpawned = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;

            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void EndGame()
    {
        if (!PhotonNetwork.InRoom)
            Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        endScreen.SetActive(true);
        roundsSurvived.text = round.ToString();
    }

    public void Restart()
    {
        if (!PhotonNetwork.InRoom)
            Time.timeScale = 1;

        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 0;

        if (!PhotonNetwork.InRoom)
            Time.timeScale = 0;

        pauseMenu.SetActive(true);
    }

    public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;

        if (!PhotonNetwork.InRoom)
            Time.timeScale = 1;

        pauseMenu.SetActive(false);
    }

    public void BackToMainMenu()
    {
        if (!PhotonNetwork.InRoom)
            Time.timeScale = 1;

        Invoke("LoadMainMenuScene", 0.4f);
    }

    public void LoadMainMenuScene()
    {
        AudioListener.volume = 1;
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (photonView.IsMine)
        {
            if (changedProps["CurrentRound"] != null)
                DisplayNextRound((int)changedProps["CurrentRound"]);
        }
    }
}