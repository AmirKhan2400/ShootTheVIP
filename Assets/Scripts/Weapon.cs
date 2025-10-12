using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] public Transform bulletFirePoint;
    [SerializeField] public WeaponData weaponData;

    public abstract void Fire();
    public abstract void Reload();
}