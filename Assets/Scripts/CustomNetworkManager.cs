using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public event Action OnClientStartHost;
    public event Action OnClientStopHost;

    public event Action OnClientConnected;
    public event Action OnClientDisconnected;

    public event Action<int> OnRoomPlayerCountChange;

    public int PlayerCount
    {
        private set
        {
            playerCount = value;
            Debug.Log("PlayerCount value changed!");
            OnRoomPlayerCountChange?.Invoke(playerCount);
        }
        get => playerCount;
    }
    private int playerCount = 0;

    [SerializeField] private List<Transform> lobbySpawnPoints = new List<Transform>();

    private int currentLobbySpawnPoint = -1;

    public override void OnStartHost()
    {
        base.OnStartHost();

        OnClientStartHost?.Invoke();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();

        PlayerCount = 0;
        OnClientStopHost?.Invoke();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.RegisterPlayer(conn.identity);
        else
            UnityEngine.Debug.LogError("Failed to register player to GameStateManager");
    }

    public override Transform GetStartPosition()
    {
        //return base.GetStartPosition();

        currentLobbySpawnPoint = (currentLobbySpawnPoint + 1) % lobbySpawnPoints.Count;
        return lobbySpawnPoints[currentLobbySpawnPoint];
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        PlayerCount++;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        PlayerCount--;
    }
}