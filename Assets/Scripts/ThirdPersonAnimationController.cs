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

    [SerializeField] private Animator animator;
    private NetworkPlayer networkPlayer;

    private List<Collider> colliders = new List<Collider>();
    private List<Rigidbody> Rigidbodies = new List<Rigidbody>();

    private Collider rootCollider;
    private Rigidbody rootRigidBody;

    private void Awake()
    {
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    private void Start()
    {
        networkPlayer.OnMovingStateChange += PlayerController_OnPlayerMoveStateChange;
        networkPlayer.OnJumpingStateChange += PlayerController_OnPlayerJump;
        networkPlayer.OnShootingStateChange += PlayerController_OnPlayerShootStateChange;
        networkPlayer.OnRunningStateChange += NetworkPlayer_OnRunningStateChange;
        //networkPlayer.OnPlayerActionHappened += PlayerController_OnPlayerActionHappened;

        rootCollider = GetComponent<Collider>();
        rootRigidBody = GetComponent<Rigidbody>();

        colliders = GetComponentsInChildren<Collider>().ToList();
        colliders.Remove(rootCollider);

        Rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
        Rigidbodies.Remove(rootRigidBody);
    }

    private void OnDestroy()
    {
        networkPlayer.OnMovingStateChange -= PlayerController_OnPlayerMoveStateChange;
        networkPlayer.OnJumpingStateChange -= PlayerController_OnPlayerJump;
        networkPlayer.OnShootingStateChange -= PlayerController_OnPlayerShootStateChange;
        networkPlayer.OnRunningStateChange -= NetworkPlayer_OnRunningStateChange;
        //networkPlayer.OnPlayerActionHappened -= PlayerController_OnPlayerActionHappened;
    }

    //private void PlayerController_OnPlayerActionHappened(PlayerController.PlayerAction action)
    //{
    //    switch (action)
    //    {
    //        case PlayerController.PlayerAction.Reload:
    //            animator.SetBool(IS_RELOADING_FLAG, true);
    //            break;

    //        case PlayerController.PlayerAction.Toss:
    //            animator.SetBool(IS_TOSS_FLAG, true);
    //            break;
    //    }
    //}

    private void PlayerController_OnPlayerMoveStateChange(bool isMoving, Vector2 MovementDirection)
    {
        float walk = isMoving ? MovementDirection.y : 0;
        float strafe = isMoving ? MovementDirection.x : 0;

        animator.SetBool(IS_MOVING_FLAG, isMoving);
        //animator.SetBool(IS_RUNNING_FLAG, playerController.IsRunning);
        animator.SetFloat(FORWARD_MOVEMENT, walk);
        animator.SetFloat(SIDE_MOVEMENT, strafe);
    }

    private void NetworkPlayer_OnRunningStateChange(bool isRunning, Vector2 movementDirection)
    {
        float walk = isRunning ? movementDirection.y : 0;
        float strafe = isRunning ? movementDirection.x : 0;

        animator.SetBool(IS_RUNNING_FLAG, isRunning);
        animator.SetFloat(FORWARD_MOVEMENT, walk);
        animator.SetFloat(SIDE_MOVEMENT, strafe);
    }

    private void PlayerController_OnPlayerJump()
    {
        animator.SetBool(IS_JUMPING_FLAG, true);
    }

    private void PlayerController_OnPlayerShootStateChange()
    {
        //animator.SetBool(IS_SHOOTING_FLAG, playerController.IsShooting);
    }
}