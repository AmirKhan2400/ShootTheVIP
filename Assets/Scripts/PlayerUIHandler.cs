using Mirror;
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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        mainCanvas.gameObject.SetActive(true);

        playerWeaponHandler.OnCurrentWeaponBulletCountChanged += PlayerWeaponHandler_OnCurrentWeaponBulletCountChanged;

        PlayerWeaponHandler_OnCurrentWeaponBulletCountChanged();

        playerHealthManager.OnHealthValueChanged += HealthManager_OnHealthValueChanged;

        HealthManager_OnHealthValueChanged();
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

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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
}