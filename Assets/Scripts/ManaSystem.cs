using System;
using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    public static ManaSystem Instance { get; private set; }

    public event Action<int, int> OnManaChanged;

    [SerializeField] private int maxMana = 3;

    private int currentMana;

    public int CurrentMana => currentMana;
    public int MaxMana => maxMana;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartTurn()
    {
        int bonus = RunManager.Instance?.CurrentRun?.GetBonusMana() ?? 0;
        SetMana(maxMana + bonus);
    }

    public bool CanAfford(int cost) => currentMana >= cost;

    public bool SpendMana(int cost)
    {
        if (!CanAfford(cost)) return false;
        SetMana(currentMana - cost);
        return true;
    }
    public void RestoreMana(int amount)
    {
        SetMana(currentMana + amount);
    }

    private void SetMana(int value)
    {
        currentMana = Mathf.Clamp(value, 0, maxMana);
        OnManaChanged?.Invoke(currentMana, maxMana);
    }
}
