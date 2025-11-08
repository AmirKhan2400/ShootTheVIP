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

    private void Start()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerRespawned += OnPlayerRespawned;
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerRespawned -= OnPlayerRespawned;
    }

    [Server]
    public void Damage(float damageValue)
    {
        if (isDead)
            return;

        Debug.Log(string.Format("Damaged: health: {0} - damageValue: {1}", HealthValue, damageValue));

        HealthValue = MathF.Max(HealthValue - damageValue, 0);

        Debug.Log(string.Format("Damaged HealthValue Updated: {0}", HealthValue, damageValue));

        DamageRPC();

        if (HealthValue == 0)
        {
            GameStateManager.Instance.SetPlayerDead(netIdentity);

            DieRPC();
        }
    }

    [ClientRpc]
    private void DieRPC()
    {
        OnDeath?.Invoke();
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

    private void OnPlayerRespawned(NetworkIdentity playerId)
    {
        if (netIdentity == playerId)
        {
            isDead = false;
            HealthValue = maxHealth;
        }
    }
}