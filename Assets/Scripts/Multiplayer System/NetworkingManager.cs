using UnityEngine;
using Photon.Pun;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    public GameObject connecting;
    public GameObject multiplayer;

    private void Start()
    {
        Debug.Log("Connecting to Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Joining Lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Ready for Multiplayer...");
        connecting.SetActive(false);
        multiplayer.SetActive(true);
    }
}