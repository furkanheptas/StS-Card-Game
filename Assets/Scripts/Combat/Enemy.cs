using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICombatEntity
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public EnemyData       Data          { get; private set; }
    public int             CurrentHP     { get; private set; }
    public int             CurrentShield { get; private set; }
    public EnemyActionData CurrentIntent { get; private set; }

    // ICombatEntity
    public int   Strength                 => statusManager.GetStrength();
    public float OutgoingDamageMultiplier => 1f;
    public IReadOnlyList<IStatusEffect> StatusEffects => statusManager.Effects;

    private readonly StatusEffectManager statusManager = new StatusEffectManager();

    public event Action<int, int>        OnHPChanged;
    public event Action<int>             OnShieldChanged;
    public event Action<EnemyActionData> OnIntentChanged;
    public event Action                  OnDied;
    public event Action                  OnStatusChanged;

    private int  actionIndex;
    private bool initialized;

    // -----------------------------------------------------------------------

    public void Configure(EnemyData enemyData)
    {
        Data = enemyData;
        Initialize();
    }

    private void Start()
    {
        if (!initialized && Data != null)
            Initialize();
    }

    private void Initialize()
    {
        if (Data == null) { Debug.LogError($"[Enemy] {name} has no EnemyData.", this); return; }

        initialized   = true;
        CurrentHP     = Data.maxHP;
        CurrentShield = 0;
        actionIndex   = 0;

        if (spriteRenderer != null && Data.sprite != null)
            spriteRenderer.sprite = Data.sprite;

        gameObject.name = Data.enemyName;
        AdvanceIntent();
    }

    // -----------------------------------------------------------------------
    // ICombatEntity

    /// <summary>Applies damage after Vulnerable modifier; triggers Thorns on attacker.</summary>
    public void TakeDamage(int amount)
    {
        int final     = statusManager.CalculateIncomingDamage(amount);
        int absorbed  = Mathf.Min(CurrentShield, final);
        CurrentShield -= absorbed;
        CurrentHP     -= final - absorbed;
        CurrentHP      = Mathf.Clamp(CurrentHP, 0, Data.maxHP);

        OnHPChanged?.Invoke(CurrentHP, Data.maxHP);
        OnShieldChanged?.Invoke(CurrentShield);

        if (CurrentHP <= 0) Die();
    }

    /// <summary>Gains shield after Frail modifier.</summary>
    public void GainShield(int amount)
    {
        CurrentShield += statusManager.CalculateBlockGain(amount);
        OnShieldChanged?.Invoke(CurrentShield);
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, Data.maxHP);
        OnHPChanged?.Invoke(CurrentHP, Data.maxHP);
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

    /// <summary>Called by CombatManager at the start of the enemy's turn.</summary>
    public void OnTurnStart()
    {
        statusManager.TickTurnStart(this);
        OnStatusChanged?.Invoke();
    }

    /// <summary>Called by CombatManager at the end of the enemy's turn.</summary>
    public void OnTurnEnd()
    {
        statusManager.TickTurnEnd(this);
        CurrentShield = 0;
        OnShieldChanged?.Invoke(CurrentShield);
        OnStatusChanged?.Invoke();
    }

    // -----------------------------------------------------------------------
    // Action

    /// <summary>Executes the current intent against the player.</summary>
    public void ExecuteAction(Player player)
    {
        if (Data.actions.Count == 0 || CurrentIntent == null) return;

        switch (CurrentIntent.type)
        {
            case EnemyActionType.Attack:
                int raw    = statusManager.CalculateOutgoingDamage(CurrentIntent.value);
                int final  = Mathf.Max(0, raw - CardEffectResolver.GlobalEnemyDamageDealtReduction);
                player?.TakeDamage(final);

                // Thorns feedback – if player has Thorns, enemy takes that damage
                int playerThorns = player != null ? player.StatusManager.GetThorns() : 0;
                if (playerThorns > 0)
                    TakeDamage(playerThorns);
                break;

            case EnemyActionType.Defend:
                GainShield(CurrentIntent.value);
                break;
        }

        actionIndex = (actionIndex + 1) % Data.actions.Count;
        AdvanceIntent();
    }

    public void ResetShieldForTurn()
    {
        CurrentShield = 0;
        OnShieldChanged?.Invoke(CurrentShield);
    }

    /// <summary>Returns Thorns stacks on this enemy.</summary>
    public int GetThorns() => statusManager.GetThorns();

    // -----------------------------------------------------------------------

    private void AdvanceIntent()
    {
        if (Data.actions.Count == 0) return;
        CurrentIntent = Data.actions[actionIndex];
        OnIntentChanged?.Invoke(CurrentIntent);
    }

    private void Die()
    {
        OnDied?.Invoke();
        Destroy(gameObject);
    }
}
