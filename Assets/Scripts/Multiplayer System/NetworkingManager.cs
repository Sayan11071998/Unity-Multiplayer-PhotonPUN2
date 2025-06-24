using UnityEngine;
using Photon.Pun;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
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
    }
}