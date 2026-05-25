using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICombatEntity
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerData data;
    [SerializeField] private PlayerView view;

    public PlayerData Data    => data;
    public int CurrentHP      { get; private set; }
    public int CurrentShield  { get; private set; }
    public int MaxHP          => data.maxHP;

    public int   Strength                 => statusManager.GetStrength();
    public float OutgoingDamageMultiplier => 1f;
    public IReadOnlyList<IStatusEffect> StatusEffects => statusManager.Effects;

    private readonly StatusEffectManager statusManager = new StatusEffectManager();

    public StatusEffectManager StatusManager => statusManager;

    public event Action<int, int> OnHPChanged;
    public event Action<int>      OnShieldChanged;
    public event Action           OnDied;
    public event Action           OnStatusChanged;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (data == null) { Debug.LogError("[Player] PlayerData is not assigned.", this); return; }

        RunData run = RunManager.Instance?.CurrentRun;
        if (run != null)
        {
            CurrentHP = run.currentHP;
            data.maxHP = run.maxHP;
        }
        else
        {
            CurrentHP = data.maxHP;
        }

        CurrentShield = 0;
        view?.Initialize(this);
    }

    public void TakeDamage(int amount)
    {
        int final     = statusManager.CalculateIncomingDamage(amount);
        int absorbed  = Mathf.Min(CurrentShield, final);
        
        Debug.Log($"[Player] Taking damage: requested={amount}, after modifiers={final}, absorbedByShield={absorbed}");
        
        CurrentShield -= absorbed;
        CurrentHP     -= final - absorbed;
        CurrentHP      = Mathf.Max(0, CurrentHP);

        Debug.Log($"[Player] HP updated: {CurrentHP + (final - absorbed)} -> {CurrentHP}");

        OnHPChanged?.Invoke(CurrentHP, MaxHP);
        OnShieldChanged?.Invoke(CurrentShield);

        if (CurrentHP <= 0)
        {
            Debug.Log("[Player] HP reached 0. Player died.");
            RunManager.Instance?.CurrentRun?.TakeDamage(int.MaxValue);
            OnDied?.Invoke();
            RunManager.Instance?.OnPlayerDeath();
        }
    }

    public void GainShield(int amount)
    {
        CurrentShield += statusManager.CalculateBlockGain(amount);
        OnShieldChanged?.Invoke(CurrentShield);
    }

    public void GainPermanentShield(int amount)
    {
        CurrentShield += amount;
        OnShieldChanged?.Invoke(CurrentShield);
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        OnHPChanged?.Invoke(CurrentHP, MaxHP);
    }

    public void ApplyStatus(IStatusEffect effect)
    {
        statusManager.Apply(effect);
        OnStatusChanged?.Invoke();
    }

    public void RemoveStatus(string id)
    {
        statusManager.Remove(id);
        OnStatusChanged?.Invoke();
    }

    public void OnTurnStart()
    {
        statusManager.TickTurnStart(this);
        OnStatusChanged?.Invoke();
    }

    public void OnTurnEnd()
    {
        statusManager.TickTurnEnd(this);
        CurrentShield = 0;
        OnShieldChanged?.Invoke(CurrentShield);
        OnStatusChanged?.Invoke();

        if (RunManager.Instance?.CurrentRun != null)
            RunManager.Instance.CurrentRun.currentHP = CurrentHP;
    }

    public int CalculateOutgoingDamage(int baseDamage) =>
        statusManager.CalculateOutgoingDamage(baseDamage);
}
