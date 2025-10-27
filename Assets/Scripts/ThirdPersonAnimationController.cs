using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdPersonAnimationController : MonoBehaviour
{
    private const string IS_JUMPING_FLAG = "IsJumping";
    private const string IS_MOVING_FLAG = "IsMoving";
    private const string IS_RUNNING_FLAG = "IsRunning";
    private const string IS_SHOOTING_FLAG = "IsFiring";
    private const string IS_RELOADING_FLAG = "IsReloading";
    private const string IS_TOSS_FLAG = "";
    private const string FORWARD_MOVEMENT = "ForwardMovement";
    private const string SIDE_MOVEMENT = "SideMovement";

    private Animator animator;
    private PlayerController playerController;

    private List<Collider> colliders = new List<Collider>();
    private List<Rigidbody> Rigidbodies = new List<Rigidbody>();

    private Collider rootCollider;
    private Rigidbody rootRigidBody;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        playerController.OnPlayerMoveStateChange += PlayerController_OnPlayerMoveStateChange;
        playerController.OnPlayerJump += PlayerController_OnPlayerJump;
        playerController.OnPlayerShootStateChange += PlayerController_OnPlayerShootStateChange;
        playerController.OnPlayerActionHappened += PlayerController_OnPlayerActionHappened;

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerDied += PlayDeathAnimation;

        rootCollider = GetComponent<Collider>();
        rootRigidBody = GetComponent<Rigidbody>();

        colliders = GetComponentsInChildren<Collider>().ToList();
        colliders.Remove(rootCollider);

        Rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
        Rigidbodies.Remove(rootRigidBody);
    }

    private void OnDestroy()
    {
        playerController.OnPlayerMoveStateChange -= PlayerController_OnPlayerMoveStateChange;
        playerController.OnPlayerJump -= PlayerController_OnPlayerJump;
        playerController.OnPlayerShootStateChange -= PlayerController_OnPlayerShootStateChange;
        playerController.OnPlayerActionHappened -= PlayerController_OnPlayerActionHappened;

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerDied -= PlayDeathAnimation;
    }

    private void PlayerController_OnPlayerActionHappened(PlayerController.PlayerAction action)
    {
        switch (action)
        {
            case PlayerController.PlayerAction.Reload:
                animator.SetBool(IS_RELOADING_FLAG, true);
                break;

            case PlayerController.PlayerAction.Toss:
                animator.SetBool(IS_TOSS_FLAG, true);
                break;
        }
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

    private void PlayerController_OnPlayerShootStateChange()
    {
        animator.SetBool(IS_SHOOTING_FLAG, playerController.IsShooting);
    }

    private void PlayDeathAnimation()
    {
        animator.enabled = false;

        rootCollider.enabled = false;
        rootRigidBody.isKinematic = true;

        //enabling ragdoll
        foreach (var collider in colliders)
            collider.enabled = true;

        foreach (var rigidbody in Rigidbodies)
            rigidbody.isKinematic = false;
    }
}