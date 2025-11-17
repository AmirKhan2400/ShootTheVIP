using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostUIManager : NetworkBehaviour
{
    private const int MINIMUM_PLAYER_COUNT = 2;

    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button startGameButton;

    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private Button sendToAllPlayerButton;

    private CustomNetworkManager networkManager;
    private bool isHost = false;

    public override void OnStartClient()
    {
        base.OnStartClient();

        networkManager = CustomNetworkManager.singleton as CustomNetworkManager;

        isHost = isServer && isClient;

        gameObject.SetActive(isHost);

        if (isHost)
            SetupUI();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isHost && networkManager != null)
            networkManager.OnRoomPlayerCountChange -= NetworkManager_OnRoomPlayerCountChange;
    }

    private void SetupUI()
    {
        networkManager.OnRoomPlayerCountChange += NetworkManager_OnRoomPlayerCountChange;
        NetworkManager_OnRoomPlayerCountChange(networkManager.PlayerCount); //manually call it to fill ui in startup

        startGameButton.onClick.AddListener(OnStartGameButtonPressed);

        sendToAllPlayerButton.onClick.AddListener(SendMessageToAllPlayer);
    }

    [Command]
    private void SendMessageToAllPlayer()
    {
        ServerMatchLogic.Instance.SendMessageToAllPlayers(messageInputField.text);
    }

    private void OnStartGameButtonPressed()
    {
        if (networkManager.PlayerCount < MINIMUM_PLAYER_COUNT)
            return;

        //start game

        DisableHostUI();

        StartMatch();
    }

    private void DisableHostUI()
    {
        gameObject.SetActive(false);
    }

    [Command]
    private void StartMatch()
    {
        ServerMatchLogic.Instance.StartMatch();
    }

    private void NetworkManager_OnRoomPlayerCountChange(int playerCount)
    {
        playerCountText.text = string.Format("{0}/{1} player", playerCount, MINIMUM_PLAYER_COUNT);

        startGameButton.interactable = playerCount >= MINIMUM_PLAYER_COUNT;
    }
}