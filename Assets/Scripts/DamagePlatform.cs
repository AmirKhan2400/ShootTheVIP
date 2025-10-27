using UnityEngine;

public class DamagePlatform : MonoBehaviour
{
    [SerializeField] private float damageAmount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))
            damageable.Damage(damageAmount);
    }
}