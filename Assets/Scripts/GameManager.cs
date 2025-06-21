using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;
    public GameObject endScreen;
    public TextMeshProUGUI roundNumber;
    public TextMeshProUGUI roundsSurvived;
    public int enemiesAlive = 0;
    public int round = 0;

    private void Update()
    {
        if (enemiesAlive == 0)
        {
            round++;
            NextWave(round);
            roundNumber.text = "Round: " + round.ToString();
        }
    }

    private void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemySpawned = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EndGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        endScreen.SetActive(true);
        roundsSurvived.text = round.ToString();
    }
}