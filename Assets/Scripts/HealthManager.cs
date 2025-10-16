using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable
{
    public float HealthValue { get => healthValue; }

    public event Action OnDamage;
    public event Action OnDeath;

    [SerializeField] private float maxHealth;

    private float healthValue;
    private bool isDead = false;

    private void Awake()
    {
        healthValue = maxHealth;
    }

    public void Damage(float damageValue)
    {
        if (isDead)
            return;

        healthValue -= damageValue;
        OnDamage?.Invoke();

        if (healthValue <= 0)
        {
            healthValue = 0;
            isDead = true;
            OnDeath?.Invoke();

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.SetPlayerDead();
        }
    }

    public void Heal(float healValue)
    {
        healthValue = Mathf.Clamp(healthValue + healValue, 0, maxHealth);
    }
}