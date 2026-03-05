using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public EnemyData Data { get; private set; }
    public int CurrentHP { get; private set; }
    public int CurrentShield { get; private set; }
    public EnemyActionData CurrentIntent { get; private set; }

    public event Action<int, int> OnHPChanged;
    public event Action<int>       OnShieldChanged;
    public event Action<EnemyActionData> OnIntentChanged;
    public event Action OnDied;

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
        if (Data == null) { Debug.LogError($"[Enemy] {name} has no EnemyData.", this); return; }

        initialized = true;
        CurrentHP    = Data.maxHP;
        CurrentShield = 0;
        actionIndex  = 0;

        if (spriteRenderer != null && Data.sprite != null)
            spriteRenderer.sprite = Data.sprite;

        gameObject.name = Data.enemyName;
        AdvanceIntent();
    }

    public void TakeDamage(int amount)
    {
        int absorbed = Mathf.Min(CurrentShield, amount);
        CurrentShield -= absorbed;
        CurrentHP     -= amount - absorbed;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, Data.maxHP);

        OnHPChanged?.Invoke(CurrentHP, Data.maxHP);
        OnShieldChanged?.Invoke(CurrentShield);

        if (CurrentHP <= 0) Die();
    }

    public void GainShield(int amount)
    {
        CurrentShield += amount;
        OnShieldChanged?.Invoke(CurrentShield);
    }

    public void ExecuteAction(Player player)
    {
        if (Data.actions.Count == 0 || CurrentIntent == null) return;

        switch (CurrentIntent.type)
        {
            case EnemyActionType.Attack: player?.TakeDamage(CurrentIntent.value); break;
            case EnemyActionType.Defend: GainShield(CurrentIntent.value);          break;
        }

        actionIndex = (actionIndex + 1) % Data.actions.Count;
        AdvanceIntent();
    }

    public void ResetShieldForTurn()
    {
        CurrentShield = 0;
        OnShieldChanged?.Invoke(CurrentShield);
    }

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
