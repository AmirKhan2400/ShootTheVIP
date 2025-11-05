using UnityEngine;

public class LobbyCameraHandler : MonoBehaviour
{
    [SerializeField] private CustomNetworkManager networkManager;
    [SerializeField] private Camera lobbyCamera;

    private void Start()
    {
        networkManager.OnClientConnected += DisableLobbyCamera;
        networkManager.OnClientStartHost += DisableLobbyCamera;

        //when player connected as host
        networkManager.OnClientStopHost += EnableLobbyCamera;
        //when player connected as client
        networkManager.OnClientDisconnected += EnableLobbyCamera;
    }

    private void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.OnClientConnected -= DisableLobbyCamera;
            networkManager.OnClientStartHost -= DisableLobbyCamera;

            networkManager.OnClientStopHost -= EnableLobbyCamera;
            networkManager.OnClientDisconnected -= EnableLobbyCamera;
        }
    }

    private void DisableLobbyCamera()
    {
        ChangeCameraState(false);
    }

    private void EnableLobbyCamera()
    {
        ChangeCameraState(true);
    }

    private void ChangeCameraState(bool state)
    {
        if (lobbyCamera.enabled == state)
            return;

        Debug.Log("NetworkManager_OnClientStartHost");

        lobbyCamera.enabled = state;
    }
}