using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive = 0;
    public int round = 0;

    public GameObject[] spawnPoints;

    public GameObject enemyPrefab;

    private void Update()
    {
        if (enemiesAlive == 0)
        {
            round++;
            NextWave(round);
        }
    }

    private void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemiesAlive++;
        }
    }
}