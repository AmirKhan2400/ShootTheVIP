using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public event Action OnWeaponAmmoChanged;

    [SerializeField] public Transform bulletFirePoint;
    [SerializeField] public WeaponData weaponData;

    public int CurrentMagBulletCount 
    { 
        set 
        {
            currentMagBulletCount = value;
            OnWeaponAmmoChanged?.Invoke();
        }
        get => currentMagBulletCount;
    }

    private int currentMagBulletCount;

    public int CurrentTotalBulletCount 
    {
        set
        {
            currentTotalBulletCount = value;
            OnWeaponAmmoChanged?.Invoke();
        }
        get => currentTotalBulletCount;
    }
    private int currentTotalBulletCount;

    public virtual void Start()
    {
        currentMagBulletCount = weaponData.bulletCountInMag;
        CurrentTotalBulletCount = weaponData.maxBulletCount - weaponData.bulletCountInMag;
    }

    public virtual void Fire()
    {
        if (currentMagBulletCount <= 0)
            return;

        CurrentMagBulletCount--;

        OnWeaponFire();
    }

    public virtual void Reload()
    {
        int requiredBulletCount = weaponData.bulletCountInMag - currentMagBulletCount;

        requiredBulletCount = Mathf.Clamp(requiredBulletCount, 0, weaponData.bulletCountInMag);

        if (requiredBulletCount == 0 || currentTotalBulletCount == 0)
            return;

        if (currentTotalBulletCount < requiredBulletCount)
            requiredBulletCount = currentTotalBulletCount;

        CurrentTotalBulletCount -= requiredBulletCount;

        CurrentMagBulletCount += requiredBulletCount;

        OnWeaponReload();
    }

    public abstract void OnWeaponReload();
    public abstract void OnWeaponFire();
}