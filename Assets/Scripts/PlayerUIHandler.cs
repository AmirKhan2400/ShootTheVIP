using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private PlayerWeaponHandler playerWeaponHandler;
    [SerializeField] private HealthManager playerHealthManager;

    [SerializeField] private TMPro.TextMeshProUGUI gunBulletCountText;
    [SerializeField] private TMPro.TextMeshProUGUI playerHealthText;

    private void Start()
    {
        playerWeaponHandler.OnCurrentWeaponBulletCountChanged += PlayerWeaponHandler_OnCurrentWeaponBulletCountChanged;

        PlayerWeaponHandler_OnCurrentWeaponBulletCountChanged();

        playerHealthManager.OnHealthValueChanged += HealthManager_OnHealthValueChanged;

        HealthManager_OnHealthValueChanged();
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