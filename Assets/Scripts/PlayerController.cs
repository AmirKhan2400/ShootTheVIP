using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //bool isMoving
    public event Action<bool> OnPlayerMoveStateChange;
    public event Action OnPlayerJump;
    public event Action OnPlayerShootStateChange;
    public event Action<PlayerAction> OnPlayerActionHappened;

    public Vector2 MovementDirection { get => movementDirection; }
    public bool IsRunning { private set; get; }
    public bool IsShooting
    {
        private set
        {
            if (isShooting != value)
            {
                isShooting = value;
                OnPlayerShootStateChange?.Invoke();
            }
        }
        get => isShooting;
    }

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float runningMultiplier = 5f;
    [SerializeField] private float mouseSensivity = 1f;
    [SerializeField] private float jumpPower = 1f;
    [SerializeField, Tooltip("Layer that consider as ground for ground check(functions like jump)")]
    private LayerMask groundMask;
    [SerializeField, Tooltip("object near to the bottom of player for ground check function")]
    private Transform groundCheck;
    [SerializeField] private Camera playerCamera;

    private Rigidbody playerRigidbody;
    private BoxCollider playerCollider;

    private PlayerWeaponHandler weaponHandler;

    private Vector2 movementDirection;
    private bool isMoving;
    private bool isRunning;
    private bool isShooting;
    private bool isDead = false;

    //mouse x-axis rotation 
    private float xRotation;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<BoxCollider>();
        weaponHandler = GetComponent<PlayerWeaponHandler>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerDied += GameStateManager_OnPlayerDied;
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerDied -= GameStateManager_OnPlayerDied;
    }

    private void Update()
    {
        if (isDead)
            return;

        HandleMouseMovement();

        HandlePlayerJump();

        HandlePlayerActions();
    }

    private void LateUpdate()
    {
        if (isDead)
            return;

        HandleMovement();
    }

    private void GameStateManager_OnPlayerDied()
    {
        isDead = true;
    }

    private void HandlePlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
        {
            playerRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            OnPlayerJump?.Invoke();
        }
    }

    //actions like shooting - tossing grenade - reloading
    private void HandlePlayerActions()
    {
        IsShooting = Input.GetMouseButtonDown(0);// || Input.GetMouseButton(0)

        if (IsShooting && weaponHandler != null)
            weaponHandler.FireCurrentWeapon();

        if (Input.GetKeyDown(KeyCode.R) && weaponHandler != null)
        {
            weaponHandler.ReloadCurrentWeapon();
            OnPlayerActionHappened?.Invoke(PlayerAction.Reload);
        }
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

        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (horizontal == 0 && vertical == 0)
        {
            UpdateMovementState(false, Vector2.zero, isRunning);
            return;
        }

        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        UpdateMovementState(true, new Vector2(horizontal, vertical), isRunning);

        float characterSpeed = isRunning ? movementSpeed * runningMultiplier : movementSpeed;

        Vector3 newPosition = playerRigidbody.position + moveDirection * characterSpeed * Time.fixedDeltaTime;
        playerRigidbody.MovePosition(newPosition);
    }

    private void UpdateMovementState(bool move, Vector2 direction, bool running)
    {
        if (isMoving == move && movementDirection == direction && IsRunning == running)
            return;

        isMoving = move;

        IsRunning = isRunning;

        movementDirection = direction;

        OnPlayerMoveStateChange?.Invoke(isMoving);
    }

    public enum PlayerAction
    {
        Reload, Toss
    }
}