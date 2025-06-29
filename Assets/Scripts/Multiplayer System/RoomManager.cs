using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    private new void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private new void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-3, 3), 2, Random.Range(-3, 3));

        if (PhotonNetwork.InRoom)
            PhotonNetwork.Instantiate("First_Person_Player", spawnPosition, Quaternion.identity);
        else
            Instantiate(Resources.Load("First_Person_Player"), spawnPosition, Quaternion.identity);
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;
}