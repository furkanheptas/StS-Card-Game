using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICombatEntity
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public EnemyData Data { get; private set; }
    public int CurrentHP { get; private set; }
    public int CurrentShield { get; private set; }
    public EnemyActionData CurrentIntent { get; private set; }

    public int Strength => statusManager.GetStrength();
    public float OutgoingDamageMultiplier => 1f;
    public IReadOnlyList<IStatusEffect> StatusEffects => statusManager.Effects;

    private readonly StatusEffectManager statusManager = new StatusEffectManager();

    public event Action<int, int> OnHPChanged;
    public event Action<int> OnShieldChanged;
    public event Action<EnemyActionData> OnIntentChanged;
    public event Action OnDied;
    public event Action OnStatusChanged;

    private int actionIndex;
    private bool initialized;

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
        if (Data == null)
        {
            Debug.LogError($"[Enemy] {name} has no EnemyData.", this);
            return;
        }

        initialized = true;
        CurrentHP = Data.maxHP;
        CurrentShield = 0;
        actionIndex = 0;

        if (spriteRenderer != null && Data.sprite != null)
            spriteRenderer.sprite = Data.sprite;

        gameObject.name = Data.enemyName;
        AdvanceIntent();
    }

    public void TakeDamage(int amount)
    {
        int final = statusManager.CalculateIncomingDamage(amount);
        int absorbed = Mathf.Min(CurrentShield, final);
        int hpDamage = final - absorbed;

        CurrentShield -= absorbed;
        CurrentHP -= hpDamage;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, Data.maxHP);

        if (hpDamage > 0)
            BattleAudioManager.Instance?.PlayEnemyDamage();

        OnHPChanged?.Invoke(CurrentHP, Data.maxHP);
        OnShieldChanged?.Invoke(CurrentShield);

        if (CurrentHP <= 0)
            Die();
    }

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
    }

    public void ExecuteAction(Player player)
    {
        StartCoroutine(ExecuteActionRoutine(player));
    }

    public IEnumerator ExecuteActionRoutine(Player player)
    {
        if (Data == null)
        {
            Debug.LogError($"[Enemy] {name} has no EnemyData during ExecuteAction!");
            yield break;
        }

        if (Data.actions.Count == 0)
        {
            Debug.LogWarning($"[Enemy] {name} has no actions defined in EnemyData.");
            yield break;
        }

        if (CurrentIntent == null)
        {
            Debug.LogWarning($"[Enemy] {name} has no CurrentIntent.");
            yield break;
        }

        Debug.Log($"[Enemy] {name} executing action: {CurrentIntent.type} with value: {CurrentIntent.value}");

        switch (CurrentIntent.type)
        {
            case EnemyActionType.Attack:
                {
                    int raw = statusManager.CalculateOutgoingDamage(CurrentIntent.value);
                    int final = Mathf.Max(0, raw - CardEffectResolver.GlobalEnemyDamageDealtReduction);

                    if (player != null)
                    {
                        Debug.Log($"[Enemy] {name} attacking Player for {final} damage.");

                        BattleAudioManager.Instance?.PlayEnemyAttack();

                        yield return CombatVisualEffect.PlayAttackMotion(transform, player.transform);
                        yield return CombatVisualEffect.PlayShake(player.transform);

                        player.TakeDamage(final);

                        int playerThorns = player.StatusManager.GetThorns();

                        if (playerThorns > 0)
                        {
                            Debug.Log($"[Enemy] {name} taking {playerThorns} Thorns damage from Player.");

                            yield return CombatVisualEffect.PlayShake(transform);
                            TakeDamage(playerThorns);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[Enemy] {name} tried to attack but player is null!");
                    }

                    break;
                }

            case EnemyActionType.Defend:
                {
                    Debug.Log($"[Enemy] {name} gaining {CurrentIntent.value} shield.");

                    yield return CombatVisualEffect.PlayShake(transform);

                    GainShield(CurrentIntent.value);
                    break;
                }
        }

        actionIndex = (actionIndex + 1) % Data.actions.Count;
        AdvanceIntent();
    }

    public void ResetShieldForTurn()
    {
        CurrentShield = 0;
        OnShieldChanged?.Invoke(CurrentShield);
    }

    public int GetThorns()
    {
        return statusManager.GetThorns();
    }

    private void AdvanceIntent()
    {
        if (Data.actions.Count == 0)
            return;

        CurrentIntent = Data.actions[actionIndex];
        OnIntentChanged?.Invoke(CurrentIntent);
    }

    private void Die()
    {
        OnDied?.Invoke();
        Destroy(gameObject);
    }
}