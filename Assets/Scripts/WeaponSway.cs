using Photon.Pun;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swaySensitivity = 2f;
    
    [SerializeField] private float swayClamp = 20f;
    [SerializeField] private float swaySmoothness = 20f;

    [SerializeField] private PhotonView photonView;

    private Vector3 startPosition;
    private Vector3 nextPosition;
    private Vector3 currentVelocity = Vector3.zero;

    private void Start() => startPosition = transform.localPosition;

    private void Update()
    {
        if (photonView != null && !photonView.IsMine) return;

        float mouseX = Input.GetAxis("Mouse X") * swaySensitivity / 100 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * swaySensitivity / 100 * Time.deltaTime;

        mouseX = Mathf.Clamp(mouseX, -swayClamp, swayClamp);
        mouseY = Mathf.Clamp(mouseY, -swayClamp, swayClamp);

        nextPosition = new Vector3(mouseX, mouseY, 0);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, nextPosition + startPosition, ref currentVelocity, Time.deltaTime * swaySmoothness);
    }
}