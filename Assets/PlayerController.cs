using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float lookSpeed = 1f;
    [SerializeField] private float jumpPower = 1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Camera playerCamera;

    private Rigidbody playerRigidbody;
    private BoxCollider playerCollider;

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
    }

    private void LateUpdate()
    {
        HandleMovement();
    }

    private void HandlePlayerJump()
    {
        if (Input.GetKey(KeyCode.Space) && IsOnGround())
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

        transform.Rotate(Vector3.up * mouseX * lookSpeed);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 40f); // prevent flipping
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
            return;

        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        Vector3 newPosition = playerRigidbody.position + moveDirection * movementSpeed * Time.fixedDeltaTime;
        playerRigidbody.MovePosition(newPosition);
    }
}