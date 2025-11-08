using Mirror;
using System;
using UnityEngine;

public class PlayerWeaponHandler : NetworkBehaviour
{
    public event Action OnCurrentWeaponBulletCountChanged;
    public Weapon CurrentWeapon => currentWeapon;

    [SerializeField] private Weapon currentWeapon;

    private void Start()
    {
        currentWeapon.OnWeaponAmmoChanged += CurrentWeapon_OnWeaponAmmoChanged;

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnLocalPlayerRespawned += OnLocalPlayerRespawned;
    }    

    private void OnDestroy()
    {
        if (currentWeapon != null)
            currentWeapon.OnWeaponAmmoChanged -= CurrentWeapon_OnWeaponAmmoChanged;
    }

    private void OnLocalPlayerRespawned()
    {
        if (CurrentWeapon == null)
            return;

        CurrentWeapon.ResetAmmo();
    }

    private void CurrentWeapon_OnWeaponAmmoChanged()
    {
        if (!isLocalPlayer)
            return;
        OnCurrentWeaponBulletCountChanged?.Invoke();
    }

    public void FireCurrentWeapon()
    {
        if (!isLocalPlayer)
            return;

        if (currentWeapon != null)
            currentWeapon.Fire();
    }

    public void ReloadCurrentWeapon()
    {
        if (!isLocalPlayer)
            return;
        if (currentWeapon != null)
            currentWeapon.Reload();
    }
}