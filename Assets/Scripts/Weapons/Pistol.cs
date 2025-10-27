using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pistol : Weapon
{
    [SerializeField] private Animator animator;
    private Camera playerCamera;
    private List<ParticleSystem> muzzleFlashParticles;

    private static readonly int ShootHash = Animator.StringToHash("Pistol Shoot");
    private static readonly int ReloadHash = Animator.StringToHash("Pistol Reload");

    public override void Start()
    {
        base.Start();

        playerCamera = Camera.main;

        muzzleFlashParticles = bulletFirePoint.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    private void PlayShootParticles()
    {
        foreach (var particle in muzzleFlashParticles)
            particle.Play();
    }

    public override void OnWeaponReload()
    {
        animator.Play(ReloadHash);
    }

    public override void OnWeaponFire()
    {
        Debug.Log("Pistol Fire Called");

        animator.Play(ShootHash);

        PlayShootParticles();

        Vector3 bulletShootPoint = playerCamera.transform.position;

        if (Physics.Raycast(bulletShootPoint, playerCamera.transform.forward, out RaycastHit hitInfo, weaponData.bulletFireRange))
        {
            Debug.Log("Pistol bullet hit something");
            if (hitInfo.transform.TryGetComponent(out IDamageable damagable))
            {
                Debug.Log("Pistol bullet hit a IDamageable!");
                damagable.Damage(weaponData.bulletDamage);
            }
        }
    }
}