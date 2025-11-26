using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("Layer that consider as ground for ground check(functions like jump)")]
    private LayerMask groundMask;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float runningMultiplier = 5f;
    [SerializeField] private float mouseSensivity = 1f;
    [SerializeField] private float jumpPower = 1f;
    [SerializeField] private Transform groundCheck;

    private Rigidbody playerRigidbody;
    private BoxCollider playerCollider;

    private Vector2 movementDirection;
    private float xRotation;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<BoxCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseMovement();

        HandlePlayerJump();

        HandleResetYPos();
    }

    private void HandleResetYPos()
    {
        if (Input.GetKey(KeyCode.Y))
        {
            var currentPos = transform.position;
            currentPos.y = 1;
            transform.position = currentPos;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandlePlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
            playerRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    private bool IsOnGround()
    {
        return Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
    }

    private void HandleMouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(mouseSensivity * mouseX * Vector3.up);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -40f, 40f); // prevent flipping
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");//when player press A/D
        float vertical = Input.GetAxisRaw("Vertical");//when player press S/W

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        float characterSpeed = isRunning ? movementSpeed * runningMultiplier : movementSpeed;

        Vector3 targetVelocity = moveDirection * characterSpeed;
        Vector3 velocity = playerRigidbody.linearVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);

        playerRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}