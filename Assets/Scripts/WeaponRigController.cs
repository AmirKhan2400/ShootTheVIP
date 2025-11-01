using UnityEngine;

public class WeaponRigController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("Sway Settings")]
    public float swayAmount = 1.5f;
    public float swaySmooth = 8f;

    [Header("Bob Settings")]
    public float bobSpeed = 6f;
    public float bobRunningSpeed = 18f;
    public float bobAmount = 0.05f;
    public float bobSmooth = 8f;

    private Vector3 originalLocalPos;
    private float bobTimer;

    private bool isCharacterMoving = false;
    private bool isPlayerDead = false;

    private void Start()
    {
        originalLocalPos = transform.localPosition;

        if (playerController != null)
            playerController.OnPlayerMoveStateChange += PlayerController_OnPlayerMoveStateChange;

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerDied += GameStateManager_OnPlayerDied;
    }

    private void GameStateManager_OnPlayerDied()
    {
        isPlayerDead = true;
    }

    private void OnDestroy()
    {
        if (playerController != null)
            playerController.OnPlayerMoveStateChange -= PlayerController_OnPlayerMoveStateChange;

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerDied -= GameStateManager_OnPlayerDied;

    }

    private void PlayerController_OnPlayerMoveStateChange(bool isMoving)
    {
        this.isCharacterMoving = isMoving;
    }

    private void Update()
    {
        if (isPlayerDead)
            return;

        HandleSway();
        HandleBob();
    }

    private void HandleSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion targetRotation = Quaternion.Euler(
            -mouseY * swayAmount,
            mouseX * swayAmount,
            0
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * swaySmooth
        );
    }

    void HandleBob()
    {
        if (isCharacterMoving)
        {
            float currentBobSpeed = playerController.IsRunning ? bobRunningSpeed : bobSpeed;

            bobTimer += Time.deltaTime * currentBobSpeed;
            Vector3 newPos = originalLocalPos + new Vector3(
                0,
                Mathf.Sin(bobTimer) * bobAmount,
                0
            );
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                newPos,
                Time.deltaTime * bobSmooth
            );
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                originalLocalPos,
                Time.deltaTime * bobSmooth
            );
            bobTimer = 0f;
        }
    }
}