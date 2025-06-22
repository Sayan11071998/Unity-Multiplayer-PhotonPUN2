using UnityEngine;

public class TouchPlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    public float cameraSensitivity = 0.1f;
    public Transform characterCamera;

    private Vector3 velocity;

    private int leftfingerId;
    private int rightfingerId;
    private float halfScreenWidth;

    private Vector2 moveInput;
    private Vector2 moveTouchStartPosition;

    private Vector2 lookInput;
    private float cameraPitch;

    private void Start()
    {
        leftfingerId = -1;
        rightfingerId = -1;
        halfScreenWidth = Screen.width / 2;
    }

    private void Update()
    {
        GetTouchInput();

        if (leftfingerId != -1)
        {
            Move();
        }

        if (rightfingerId != -1)
        {
            LookAround();
        }
    }

    void GetTouchInput()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);

                if (t.phase == TouchPhase.Began)
                {
                    if (t.position.x < halfScreenWidth && leftfingerId == -1)
                    {
                        leftfingerId = t.fingerId;
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x >= halfScreenWidth && rightfingerId == -1)
                    {
                        rightfingerId = t.fingerId;
                    }
                }

                if (t.phase == TouchPhase.Canceled)
                {

                }

                if (t.phase == TouchPhase.Moved)
                {
                    if (leftfingerId == t.fingerId)
                    {
                        moveInput = t.position - moveTouchStartPosition;
                    }
                    else if (rightfingerId == t.fingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                }

                if (t.phase == TouchPhase.Stationary)
                {
                    if (t.fingerId == rightfingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                }

                if (t.phase == TouchPhase.Ended)
                {
                    if (leftfingerId == t.fingerId)
                    {
                        leftfingerId = -1;
                    }
                    else if (rightfingerId == t.fingerId)
                    {
                        rightfingerId = -1;
                    }
                }
            }
        }
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * moveInput.normalized.x + transform.forward * moveInput.normalized.y;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void LookAround()
    {
        cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
        characterCamera.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(transform.up * lookInput.x);
    }
}