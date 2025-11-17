using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : NetworkBehaviour
{
    [SerializeField] private PlayerWeaponHandler playerWeaponHandler;
    [SerializeField] private HealthManager playerHealthManager;

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private TextMeshProUGUI gunBulletCountText;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI playerObjectiveText;
    [SerializeField] private TextMeshProUGUI gameScoreboardText;

    [SerializeField] private Image pauseScreen;

    [SerializeField] private Image deathScreen;
    [SerializeField] private TextMeshProUGUI remainingRespawnText;
    [SerializeField] private Button respawnButton;

    private const string objectiveTextAnimationID = "PlayerUIHandler_objectiveTextAnimationID";

    private const string RemainigRespawnText = "You have only {0} live(s) left.";

    public override void OnStartClient()
    {
        base.OnStartClient();

        mainCanvas.gameObject.SetActive(isLocalPlayer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseScreen();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        playerWeaponHandler.OnCurrentWeaponBulletCountChanged += PlayerWeaponHandler_OnCurrentWeaponBulletCountChanged;

        PlayerWeaponHandler_OnCurrentWeaponBulletCountChanged();

        playerHealthManager.OnHealthValueChanged += HealthManager_OnHealthValueChanged;

        HealthManager_OnHealthValueChanged();

        InitializeDeathScreen();
    }

    [ClientRpc]
    public void ShowObjectiveTextRPC(string text, float duration)
    {
        ShowObjectiveText(text, duration);
    }

    [ClientRpc]
    public void UpdateScoreboard(int assassinScore, int bodyguardScore)
    {
        gameScoreboardText.text = string.Format("{0}-{1}", assassinScore, bodyguardScore);
    }

    public void ShowObjectiveText(string text, float duration)
    {
        if (!isLocalPlayer)
            return;

        DOTween.Kill(objectiveTextAnimationID);

        playerObjectiveText.DOFade(0f, 0f);

        playerObjectiveText.text = text;

        Sequence sequence = DOTween.Sequence().SetId(objectiveTextAnimationID);
        sequence.Append(playerObjectiveText.DOFade(1f, 0f));
        sequence.AppendInterval(duration);
        sequence.Append(playerObjectiveText.DOFade(0f, .5f));
    }

    private void TogglePauseScreen()
    {
        bool isPaused = !pauseScreen.gameObject.activeSelf;

        pauseScreen.gameObject.SetActive(isPaused);

        ChangeMouseVisibility(isPaused);
    }

    private void InitializeDeathScreen()
    {
        GameStateManager.Instance.OnLocalPlayerDied += ShowDeathScreen;

        respawnButton.onClick.AddListener(OnRespawnButtonPressed);
    }

    private void ShowDeathScreen(bool canRespawn)
    {
        UpdateDeathScreen();

        ChangeMouseVisibility(true);
        deathScreen.gameObject.SetActive(true);
        respawnButton.interactable = canRespawn;
    }

    private void UpdateDeathScreen()
    {
        if (TryGetComponent(out PlayerState playerState) && playerState.GetRespawnCountForText(out int remainingSpawn))
        {
            //this is not a good solution but i couldn't find any better way for now
            //(this value is not updated when i need it to show, so i do it manually)
            remainingSpawn = Mathf.Clamp(remainingSpawn - 1, 0, int.MaxValue);

            remainingRespawnText.text = string.Format(RemainigRespawnText, remainingSpawn);
        }
        else
            remainingRespawnText.text = string.Empty;
    }

    private void OnRespawnButtonPressed()
    {
        respawnButton.interactable = false;
        deathScreen.gameObject.SetActive(false);
        ChangeMouseVisibility(false);
        RequestRespawn();
    }

    [Command]
    private void RequestRespawn()
    {
        GameStateManager.Instance.RespawnDeadPlayer(netIdentity);
    }

    private void HealthManager_OnHealthValueChanged()
    {
        playerHealthText.text = string.Format("{0}", playerHealthManager.HealthValue);
    }

    private void PlayerWeaponHandler_OnCurrentWeaponBulletCountChanged()
    {
        int currentMagBulletCount = playerWeaponHandler.CurrentWeapon.CurrentMagBulletCount;
        int weaponCurrentTotalBulletCount = playerWeaponHandler.CurrentWeapon.CurrentTotalBulletCount;

        gunBulletCountText.text = string.Format("{0} | {1}", currentMagBulletCount, weaponCurrentTotalBulletCount);
    }

    private void ChangeMouseVisibility(bool visible)
    {
        Cursor.visible = visible;

        if (visible)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }
}