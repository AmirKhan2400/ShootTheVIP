using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public float bulletFireRange;
    public float bulletDamage;

    [Tooltip("how many bullet can user have after buy ammo for this gun")]
    public int maxBulletCount;
    [Tooltip("how many bullet exists in one mag for this gun")]
    public int bulletCountInMag;
}
