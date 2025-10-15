using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public const string IS_JUMPING_FLAG = "IsJumping";
    private const string IS_MOVING_FLAG = "IsMoving";
    private const string IS_RUNNING_FLAG = "IsRunning";
    private const string FORWARD_MOVEMENT = "ForwardMovement";
    private const string SIDE_MOVEMENT = "SideMovement";

    private Animator animator;
    private PlayerController playerController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        playerController.OnPlayerMoveStateChange += PlayerController_OnPlayerMoveStateChange;
        playerController.OnPlayerJump += PlayerController_OnPlayerJump;
    }

    private void OnDestroy()
    {
        playerController.OnPlayerMoveStateChange -= PlayerController_OnPlayerMoveStateChange;
        playerController.OnPlayerJump -= PlayerController_OnPlayerJump;
    }

    private void PlayerController_OnPlayerMoveStateChange(bool isMoving)
    {
        var direction = playerController.MovementDirection;

        float walk = isMoving ? direction.y : 0;
        float strafe = isMoving ? direction.x : 0;

        animator.SetBool(IS_MOVING_FLAG, isMoving);
        animator.SetBool(IS_RUNNING_FLAG, playerController.IsRunning);
        animator.SetFloat(FORWARD_MOVEMENT, walk);
        animator.SetFloat(SIDE_MOVEMENT, strafe);
    }

    private void PlayerController_OnPlayerJump()
    {
        animator.SetBool(IS_JUMPING_FLAG, true);
    }
}