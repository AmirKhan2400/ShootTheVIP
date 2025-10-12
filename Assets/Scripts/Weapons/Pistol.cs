using UnityEngine;

public class Pistol : Weapon
{
    public override void Fire()
    {
        //Debug.Log("Pistol Fire Called");
        if (Physics.Raycast(bulletFirePoint.position, bulletFirePoint.forward, out RaycastHit hitInfo, weaponData.bulletFireRange))
        {
            //Debug.Log("Pistol bullet hit something");
            if (hitInfo.transform.TryGetComponent(out HealthManager health))
            {
                //Debug.Log("Pistol bullet hit a HealthManager!");
                health.Damage(weaponData.bulletDamage);
            }
        }
    }

    public override void Reload()
    {

    }
}