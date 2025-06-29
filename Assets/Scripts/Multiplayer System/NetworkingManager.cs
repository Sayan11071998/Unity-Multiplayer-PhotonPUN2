using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject connecting;
    [SerializeField] private GameObject multiplayer;

    private void Start() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        connecting.SetActive(false);
        multiplayer.SetActive(true);
    }

    public void FindMatch() => PhotonNetwork.JoinRandomRoom();

    public override void OnJoinRandomFailed(short returnCode, string message) => MakeRoom();

    private void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);

        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 6,
            PublishUserId = true
        };

        PhotonNetwork.CreateRoom("RoomName_" + randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom() => PhotonNetwork.LoadLevel(2);
}