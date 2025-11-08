using Mirror;
using System;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
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
    [SyncVar(hook = nameof(OnMagAmmoChanged))]
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
    [SyncVar(hook = nameof(OnTotalAmmoChanged))]
    private int currentTotalBulletCount;

    public abstract Transform BulletFireTransform { get; }

    public override void OnStartServer()
    {
        base.OnStartServer();

        ResetAmmo();
    }

    private void OnMagAmmoChanged(int oldValue, int newValue)
    {
        OnWeaponAmmoChanged?.Invoke();
    }

    private void OnTotalAmmoChanged(int oldValue, int newValue)
    {
        OnWeaponAmmoChanged?.Invoke();
    }

    [Command]
    public virtual void Fire()
    {
        if (currentMagBulletCount <= 0)
            return;

        CurrentMagBulletCount--;

        Vector3 bulletShootPoint = BulletFireTransform.position;

        if (Physics.Raycast(bulletFirePoint.position, bulletFirePoint.forward, out RaycastHit hitInfo, weaponData.bulletFireRange))
            if (hitInfo.collider.TryGetComponent(out IDamageable damagable))
                damagable.Damage(weaponData.bulletDamage);

        FireRPC();
    }

    [ClientRpc]
    public void FireRPC()
    {
        OnWeaponFire();
    }

    [Command]
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

        ReloadRPC();
    }

    [ClientRpc]
    public void ReloadRPC()
    {
        OnWeaponReload();
    }

    public abstract void OnWeaponReload();
    public abstract void OnWeaponFire();

    public void ResetAmmo()
    {
        currentMagBulletCount = weaponData.bulletCountInMag;
        CurrentTotalBulletCount = weaponData.maxBulletCount - weaponData.bulletCountInMag;
    }
}