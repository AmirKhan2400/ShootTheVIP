using System;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public event Action OnCurrentWeaponBulletCountChanged;
    public Weapon CurrentWeapon => currentWeapon;

    [SerializeField] private Weapon currentWeapon;

    private void Start()
    {
        currentWeapon.OnWeaponAmmoChanged += CurrentWeapon_OnWeaponAmmoChanged;
    }

    private void OnDestroy()
    {
        if (currentWeapon != null)
            currentWeapon.OnWeaponAmmoChanged -= CurrentWeapon_OnWeaponAmmoChanged;
    }

    private void CurrentWeapon_OnWeaponAmmoChanged()
    {
        OnCurrentWeaponBulletCountChanged?.Invoke();
    }

    public void FireCurrentWeapon()
    {
        if (currentWeapon != null)
            currentWeapon.Fire();
    }

    public void ReloadCurrentWeapon()
    {
        if (currentWeapon != null)
            currentWeapon.Reload();
    }
}