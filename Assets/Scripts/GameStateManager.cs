using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public event System.Action OnPlayerDied;

    [Header("Game State")]
    public bool IsPlayerAlive = true;

    public void SetPlayerDead()
    {
        IsPlayerAlive = false;
        OnPlayerDied?.Invoke();
    }
}