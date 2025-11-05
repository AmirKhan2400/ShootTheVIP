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
}