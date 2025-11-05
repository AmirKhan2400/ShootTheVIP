using Mirror;
using System;
using UnityEngine;

public class HealthManager : NetworkBehaviour, IDamageable
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

    [SyncVar(hook = nameof(OnHealthValueChangedThroughNetwork))]
    private float healthValue;

    [SyncVar]
    private bool isDead = false;

    private void Awake()
    {
        HealthValue = maxHealth;
    }

    [Server]
    public void Damage(float damageValue)
    {
        if (isDead)
            return;

        HealthValue = MathF.Max(HealthValue - damageValue, 0);

        DamageRPC();

        if (HealthValue == 0)
            DieRPC();
    }

    [ClientRpc]
    private void DieRPC()
    {
        OnDeath?.Invoke();

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetPlayerDead();
    }

    [ClientRpc]
    private void DamageRPC()
    {
        OnDamage?.Invoke();
    }

    [Command]
    public void Heal(float healValue)
    {
        HealRPC(healValue);
    }

    [ClientRpc]
    private void HealRPC(float healValue)
    {
        HealthValue = Mathf.Clamp(HealthValue + healValue, 0, maxHealth);
    }

    private void OnHealthValueChangedThroughNetwork(float oldValue, float newValue)
    {
        OnHealthValueChanged?.Invoke();
    }
}