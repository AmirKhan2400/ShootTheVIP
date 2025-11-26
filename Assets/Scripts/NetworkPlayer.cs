using Mirror;
using System;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public event Action<bool, Vector2> OnMovingStateChange;
    public event Action OnShootingStateChange;
    public event Action<bool, Vector2> OnRunningStateChange;
    public event Action OnJumpingStateChange;

    private OnlinePlayerController localController;

    [SyncVar(hook = nameof(OnMovingStateChangeHook))] private bool isMoving;
    [SyncVar(hook = nameof(OnRunningStateChangeHook))] private bool isRunning;
    [SyncVar(hook = nameof(OnShootingStateChangeHook))] private bool isShooting;
    [SyncVar(hook = nameof(OnJumpingStateChangeHook))] private bool isJumping;
    [SyncVar] private Vector2 MovementDirection;

    public override void OnStartLocalPlayer()
    {
        if (TryGetComponent(out localController))
        {
            // Subscribe to events
            localController.OnPlayerMoveStateChange += OnLocalMoveChanged;
            localController.OnPlayerJump += OnLocalJump;
            localController.OnPlayerShootStateChange += OnLocalShoot;
        }
    }

    private void OnLocalMoveChanged(bool moving)
    {
        isMoving = moving;
        isRunning = localController.IsRunning;
        MovementDirection = localController.MovementDirection;
        CmdSetMoveState(isMoving, isRunning);
    }

    #region Hook Methods
    private void OnMovingStateChangeHook(bool oldValue, bool newValue)
    {
        if (oldValue == newValue)
            return;

        OnMovingStateChange?.Invoke(newValue, MovementDirection);
    }

    private void OnShootingStateChangeHook(bool oldValue, bool newValue)
    {
        if (oldValue == newValue)
            return;

        OnShootingStateChange?.Invoke();
    }

    private void OnRunningStateChangeHook(bool oldValue, bool newValue)
    {
        if (oldValue == newValue)
            return;

        OnRunningStateChange?.Invoke(newValue, MovementDirection);
    }

    private void OnJumpingStateChangeHook(bool oldValue, bool newValue)
    {
        if (oldValue == newValue)
            return;

        OnJumpingStateChange?.Invoke();
    }

    #endregion


    private void OnLocalShoot()
    {
        isShooting = localController.IsShooting;
        CmdSetShootState(isShooting);
    }

    private void OnLocalJump()
    {
        isJumping = true;
        CmdSetJumpState(isJumping);
        Invoke(nameof(ResetJump), 0.2f);
    }

    private void ResetJump() => isJumping = false;

    [Command]
    private void CmdSetMoveState(bool moving, bool running)
    {
        isMoving = moving;
        isRunning = running;
    }

    [Command]
    private void CmdSetShootState(bool shooting)
    {
        isShooting = shooting;
    }

    [Command]
    private void CmdSetJumpState(bool jumping)
    {
        isJumping = jumping;
    }
}
