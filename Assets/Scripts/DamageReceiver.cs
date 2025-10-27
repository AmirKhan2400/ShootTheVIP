using UnityEngine;

public class DamageReceiver : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private float damageMultiplier = 1;

    public void Damage(float DamageValue)
    {
        if (healthManager != null)
            healthManager.Damage(DamageValue * damageMultiplier);
    }
}