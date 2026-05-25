using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private const float EnemyActionDelay   = 1f;
    private const float MessageDisplayTime = 1.5f;

    private bool isEnemyTurnActive = false;
    private GameObject handObject;

    private void Awake()
    {
        handObject = GameObject.Find("Hand");

        if (handObject == null)
            Debug.LogWarning("[CombatManager] Could not find a GameObject named 'Hand' in the scene.");
    }

    private void Start()
    {
        CardEffectResolver.ResetCombatModifiers();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (DeckManager.Instance != null)
        {
            DeckManager.Instance.OnTurnEnded += OnPlayerTurnEnded;
            Debug.Log("[CombatManager] Subscribed to DeckManager.OnTurnEnded.");
        }
        else
        {
            Debug.LogError("[CombatManager] DeckManager.Instance is null in Start(). " +
                           "Enemy turn will never trigger. Check Script Execution Order.");
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (DeckManager.Instance != null)
            DeckManager.Instance.OnTurnEnded -= OnPlayerTurnEnded;
    }

    private void OnPlayerTurnEnded()
    {
        Debug.Log("[CombatManager] Player turn ended. Starting enemy turn...");

        if (isEnemyTurnActive)
        {
            Debug.LogWarning("[CombatManager] Enemy turn already running, ignoring duplicate call.");
            return;
        }

        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        isEnemyTurnActive = true;
        Debug.Log("[CombatManager] EnemyTurnRoutine started.");

        SetHandActive(false);

        yield return StartCoroutine(TryShowMessage("DÜŞMANIN SIRASI", MessageDisplayTime));

        List<Enemy> enemies = GetLivingEnemiesSnapshot();
        Debug.Log($"[CombatManager] {enemies.Count} enemy(ies) will act this turn.");

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
            {
                Debug.LogWarning("[CombatManager] Skipping null enemy in list.");
                continue;
            }

            Debug.Log($"[CombatManager] Enemy '{enemy.name}' turn start.");
            enemy.OnTurnStart();

            yield return new WaitForSeconds(EnemyActionDelay);

            if (Player.Instance == null)
            {
                Debug.LogError("[CombatManager] Player.Instance is null, cannot execute enemy action.");
            }
            else
            {
                Debug.Log($"[CombatManager] Calling ExecuteAction on '{enemy.name}'.");
                enemy.ExecuteAction(Player.Instance);
            }

            enemy.OnTurnEnd();
            Debug.Log($"[CombatManager] Enemy '{enemy.name}' turn end.");
        }

        yield return StartCoroutine(TryShowMessage("SIRA SENDE!", MessageDisplayTime));

        SetHandActive(true);

        if (Player.Instance != null)
        {
            Player.Instance.OnTurnStart();
            Debug.Log("[CombatManager] Player turn started.");
        }
        else
        {
            Debug.LogError("[CombatManager] Player.Instance is null, cannot start player turn.");
        }

        isEnemyTurnActive = false;
        Debug.Log("[CombatManager] EnemyTurnRoutine finished.");
    }

    private List<Enemy> GetLivingEnemiesSnapshot()
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogError("[CombatManager] BattleManager.Instance is null! Falling back to FindObjectsByType.");
            return new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        }

        List<Enemy> snapshot = new List<Enemy>(BattleManager.Instance.ActiveEnemies);
        Debug.Log($"[CombatManager] BattleManager.ActiveEnemies count: {snapshot.Count}");
        return snapshot;
    }

    private IEnumerator TryShowMessage(string message, float duration)
    {
        if (BattleUIBroadcast.Instance != null)
            yield return StartCoroutine(BattleUIBroadcast.Instance.ShowMessageRoutine(message, duration));
        else
            yield return new WaitForSeconds(duration);
    }

    private void SetHandActive(bool active)
    {
        if (handObject == null) return;
        handObject.SetActive(active);
        Debug.Log($"[CombatManager] Hand active: {active}");
    }
}
