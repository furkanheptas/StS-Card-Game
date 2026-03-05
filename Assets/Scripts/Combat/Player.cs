using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerData data;
    [SerializeField] private PlayerView view;

    public PlayerData Data => data;
    public int CurrentHP { get; private set; }
    public int CurrentShield { get; private set; }
    public int MaxHP => data.maxHP;

    public event Action<int, int> OnHPChanged;
    public event Action<int> OnShieldChanged;
    public event Action OnDied;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError("[Player] PlayerData is not assigned.", this);
            return;
        }

        CurrentHP = data.maxHP;
        CurrentShield = 0;
        view?.Initialize(this);
    }

    public void TakeDamage(int amount)
    {
        int absorbed = Mathf.Min(CurrentShield, amount);
        CurrentShield -= absorbed;
        CurrentHP -= amount - absorbed;
        CurrentHP = Mathf.Max(0, CurrentHP);

        OnHPChanged?.Invoke(CurrentHP, data.maxHP);
        OnShieldChanged?.Invoke(CurrentShield);

        if (CurrentHP <= 0) OnDied?.Invoke();
    }

    public void GainShield(int amount)
    {
        CurrentShield += amount;
        OnShieldChanged?.Invoke(CurrentShield);
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, data.maxHP);
        OnHPChanged?.Invoke(CurrentHP, data.maxHP);
    }
    public void OnTurnEnd()
    {
        CurrentShield = 0;
        OnShieldChanged?.Invoke(CurrentShield);
    }
}
