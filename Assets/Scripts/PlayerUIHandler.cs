using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : NetworkBehaviour
{
    [SerializeField] private PlayerWeaponHandler playerWeaponHandler;
    [SerializeField] private HealthManager playerHealthManager;

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI gunBulletCountText;
    [SerializeField] private TMPro.TextMeshProUGUI playerHealthText;

    [SerializeField] private Image pauseScreen;

    [SerializeField] private Image deathScreen;
    [SerializeField] private Button respawnButton;

    public override void OnStartClient()
    {
        base.OnStartClient();

        mainCanvas.gameObject.SetActive(isLocalPlayer);
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

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseScreen();
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

    private void ShowDeathScreen()
    {
        ChangeMouseVisibility(true);
        deathScreen.gameObject.SetActive(true);
        respawnButton.interactable = true;
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