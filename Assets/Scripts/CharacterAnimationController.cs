using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
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
    }

    private void OnDestroy()
    {
        playerController.OnPlayerMoveStateChange -= PlayerController_OnPlayerMoveStateChange;
    }

    private void PlayerController_OnPlayerMoveStateChange(bool isMoving)
    {
        var direction = playerController.MovementDirection;

        float walk = isMoving ? direction.y : 0;
        float strafe = isMoving ? direction.x : 0;

        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("Walk", walk);
        animator.SetFloat("Strafe", strafe);
    }
}