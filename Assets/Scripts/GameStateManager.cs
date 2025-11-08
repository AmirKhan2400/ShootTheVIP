using Mirror;
using System;
using UnityEngine;

public class GameStateManager : NetworkSingleton<GameStateManager>
{
    public event Action<NetworkIdentity> OnPlayerDied;  // triggered on all clients when someone dies
    public event Action<NetworkIdentity> OnPlayerRespawned;  // triggered on all clients when someone respawn

    public event Action OnLocalPlayerDied;              // only for local client UI
    public event Action OnLocalPlayerRespawned;         // only for local client UI     

    [SyncVar]
    public bool isGameRunning = true;

    // Server-tracked list of players (alive/dead)
    private readonly SyncList<NetworkIdentity> alivePlayers = new();
    private readonly SyncList<NetworkIdentity> deadPlayers = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        alivePlayers.Clear();
        deadPlayers.Clear();
    }

    [Server]
    public void RegisterPlayer(NetworkIdentity playerIdentity)
    {
        if (!alivePlayers.Contains(playerIdentity))
            alivePlayers.Add(playerIdentity);
    }

    [Server]
    public void SetPlayerDead(NetworkIdentity playerIdentity)
    {
        if (!alivePlayers.Contains(playerIdentity))
            return;

        alivePlayers.Remove(playerIdentity);
        deadPlayers.Add(playerIdentity);

        RpcNotifyPlayerDied(playerIdentity);
    }

    [Server]
    public void RespawnDeadPlayer(NetworkIdentity playerIdentity)
    {
        if (!deadPlayers.Contains(playerIdentity))
            return;

        deadPlayers.Remove(playerIdentity);
        alivePlayers.Add(playerIdentity);

        playerIdentity.gameObject.GetComponent<NetworkTransformReliable>()
            .ServerTeleport(new Vector3(500, 1, 500), Quaternion.identity);

        RpcNotifyPlayerRespawn(playerIdentity);
    }

    [ClientRpc]
    private void RpcNotifyPlayerDied(NetworkIdentity deadPlayer)
    {
        Debug.Log($"[GameStateManager] Player died: {deadPlayer.name}");
        OnPlayerDied?.Invoke(deadPlayer);

        // if this is *our* player, trigger local death event
        if (deadPlayer.isLocalPlayer)
        {
            OnLocalPlayerDied?.Invoke();
        }
        else
        {
            deadPlayer.gameObject.SetActive(false);
        }
    }

    [ClientRpc]
    private void RpcNotifyPlayerRespawn(NetworkIdentity respawnPlayerID)
    {
        Debug.Log($"[GameStateManager] Player respawned: {respawnPlayerID.name}");
        OnPlayerRespawned?.Invoke(respawnPlayerID);

        if (respawnPlayerID.isLocalPlayer)
        {
            OnLocalPlayerRespawned?.Invoke();
        }
        else
        {
            respawnPlayerID.gameObject.SetActive(true);
        }
    }
}