using Photon.Pun;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;

    public float mouseSensitivity = 100f;

    public PhotonView photonView;

    private float xRotation = 0f;

    private void Start() => Cursor.lockState = CursorLockMode.Locked;

    private void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}