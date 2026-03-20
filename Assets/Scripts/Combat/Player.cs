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

    // ICombatEntity
    public int   Strength                 => statusManager.GetStrength();
    public float OutgoingDamageMultiplier => 1f;
    public IReadOnlyList<IStatusEffect> StatusEffects => statusManager.Effects;

    private readonly StatusEffectManager statusManager = new StatusEffectManager();

    /// <summary>Direct access to the player's StatusEffectManager (read-only intended for external systems).</summary>
    public StatusEffectManager StatusManager => statusManager;

    public event Action<int, int> OnHPChanged;
    public event Action<int>      OnShieldChanged;
    public event Action           OnDied;
    public event Action           OnStatusChanged;

    // -----------------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (data == null) { Debug.LogError("[Player] PlayerData is not assigned.", this); return; }
        CurrentHP     = data.maxHP;
        CurrentShield = 0;
        view?.Initialize(this);
    }

    // -----------------------------------------------------------------------
    // ICombatEntity

    /// <summary>Applies damage after Vulnerable modifier; updates HP and Shield.</summary>
    public void TakeDamage(int amount)
    {
        int final     = statusManager.CalculateIncomingDamage(amount);
        int absorbed  = Mathf.Min(CurrentShield, final);
        CurrentShield -= absorbed;
        CurrentHP     -= final - absorbed;
        CurrentHP      = Mathf.Max(0, CurrentHP);

        OnHPChanged?.Invoke(CurrentHP, MaxHP);
        OnShieldChanged?.Invoke(CurrentShield);

        if (CurrentHP <= 0) OnDied?.Invoke();
    }

    /// <summary>Gains shield after Frail modifier.</summary>
    public void GainShield(int amount)
    {
        CurrentShield += statusManager.CalculateBlockGain(amount);
        OnShieldChanged?.Invoke(CurrentShield);
    }

    /// <summary>Gains shield that persists across turns (Barricade / retain shield effect).</summary>
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

    // -----------------------------------------------------------------------
    // Turn hooks

    /// <summary>Called by CombatManager at the very start of the player's turn.</summary>
    public void OnTurnStart()
    {
        statusManager.TickTurnStart(this);
        OnStatusChanged?.Invoke();
    }

    /// <summary>Called by DeckManager at end of turn. Resets shield to match STS default.</summary>
    public void OnTurnEnd()
    {
        statusManager.TickTurnEnd(this);
        CurrentShield = 0;
        OnShieldChanged?.Invoke(CurrentShield);
        OnStatusChanged?.Invoke();
    }

    // -----------------------------------------------------------------------
    // Damage helper

    /// <summary>Calculates final outgoing attack damage with Strength and Weak applied.</summary>
    public int CalculateOutgoingDamage(int baseDamage) =>
        statusManager.CalculateOutgoingDamage(baseDamage);
}
