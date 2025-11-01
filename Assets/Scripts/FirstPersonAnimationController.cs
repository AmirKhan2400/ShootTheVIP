using UnityEngine;

public class FirstPersonAnimationController : MonoBehaviour
{
    private const string IS_MOVING_FLAG = "IsMoving";

    [SerializeField] private Animator animator;
    private PlayerController playerController;

    private bool isPlayerDead = false;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        playerController.OnPlayerMoveStateChange += PlayerController_OnPlayerMoveStateChange;

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerDied += Instance_OnPlayerDied;
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerDied -= Instance_OnPlayerDied;

        if (playerController != null)
            playerController.OnPlayerMoveStateChange -= PlayerController_OnPlayerMoveStateChange;
    }

    private void Instance_OnPlayerDied()
    {
        isPlayerDead = true;
    }

    private void PlayerController_OnPlayerMoveStateChange(bool isMoving)
    {
        if (isPlayerDead)
            isMoving = false;

        animator.SetBool(IS_MOVING_FLAG, isMoving);
    }
}