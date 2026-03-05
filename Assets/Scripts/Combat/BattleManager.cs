using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject enemyBasePrefab;
    [SerializeField] private EnemyHUDManager enemyHUD;

    [Header("Spawn Anchor")]
    [Tooltip("Enemy spawn center point.")]
    [SerializeField] private Transform encounterAnchor;

    [Header("Auto-Start (optional)")]
    [Tooltip("If assigned, this encounter loads automatically on Start(). Optional for testing purposes.")]
    [SerializeField] private EncounterData startingEncounter;

    public event Action<Enemy> OnEnemySpawned;
    public event Action OnAllEnemiesDefeated;

    private readonly List<Enemy> activeEnemies = new List<Enemy>();

    public IReadOnlyList<Enemy> ActiveEnemies => activeEnemies;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (startingEncounter != null)
            StartEncounter(startingEncounter);
    }

    public void StartEncounter(EncounterData encounter)
    {
        if (encounter == null) return;

        Vector3 anchor = encounterAnchor != null ? encounterAnchor.position : Vector3.zero;

        foreach (EncounterSlot slot in encounter.slots)
        {
            if (slot.enemyData == null) continue;

            Vector3 spawnPos = anchor + (Vector3)slot.positionOffset;
            Enemy enemy = SpawnEnemy(slot.enemyData, spawnPos);
        }
    }

    public Enemy SpawnEnemy(EnemyData data, Vector3 worldPosition)
    {
        if (enemyBasePrefab == null)
        {
            Debug.LogError("[BattleManager] enemyBasePrefab is not assigned.", this);
            return null;
        }

        GameObject go = Instantiate(enemyBasePrefab, worldPosition, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("[BattleManager] enemyBasePrefab has no Enemy component.", go);
            return null;
        }

        enemy.Configure(data);
        RegisterEnemy(enemy);
        return enemy;
    }

    private void RegisterEnemy(Enemy enemy)
    {
        activeEnemies.Add(enemy);
        enemy.OnDied += () => OnEnemyDied(enemy);

        enemyHUD?.RegisterEnemy(enemy);
        OnEnemySpawned?.Invoke(enemy);
    }

    private void OnEnemyDied(Enemy enemy)
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0)
            OnAllEnemiesDefeated?.Invoke();
    }
}
