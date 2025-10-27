using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable
{
    public float HealthValue 
    {
        private set
        {
            healthValue = value;
            OnHealthValueChanged?.Invoke();
        }
        get => healthValue; 
    }

    public event Action OnDamage;
    public event Action OnDeath;
    public event Action OnHealthValueChanged;

    [SerializeField] private float maxHealth;

    private float healthValue;
    private bool isDead = false;

    private void Awake()
    {
        HealthValue = maxHealth;
    }

    public void Damage(float damageValue)
    {
        if (isDead)
            return;

        HealthValue -= damageValue;
        OnDamage?.Invoke();

        if (HealthValue <= 0)
        {
            HealthValue = 0;
            isDead = true;
            OnDeath?.Invoke();

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.SetPlayerDead();
        }
    }

    public void Heal(float healValue)
    {
        HealthValue = Mathf.Clamp(HealthValue + healValue, 0, maxHealth);
    }
}