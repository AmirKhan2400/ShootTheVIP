using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    public event Action OnClientStartHost;
    public event Action OnClientStopHost;

    public event Action OnClientConnected;
    public event Action OnClientDisconnected;

    public override void OnStartHost()
    {
        base.OnStartHost();

        OnClientStartHost?.Invoke();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();

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
        base.OnServerAddPlayer(conn);

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.RegisterPlayer(conn.identity);
        else
            UnityEngine.Debug.LogError("Failed to register player to GameStateManager");
    }
}