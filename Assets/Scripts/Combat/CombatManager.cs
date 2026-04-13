using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    private bool isProcessingTurn = false;
    private GameObject handObject;

    private IEnumerator Start()
    {
        handObject = GameObject.Find("Hand");

        yield return null;

        CardEffectResolver.ResetCombatModifiers();

        if (DeckManager.Instance != null)
            DeckManager.Instance.OnTurnEnded += OnPlayerTurnEnded;

        if (BattleUIBroadcast.Instance != null)
            yield return StartCoroutine(BattleUIBroadcast.Instance.ShowMessageRoutine("DÖVÜŞ BAŞLASIN!", 2f));
    }

    private void OnPlayerTurnEnded()
    {
        if (isProcessingTurn) return;
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        isProcessingTurn = true;

        if (handObject != null)
        {
            handObject.SetActive(false);
            Debug.Log("KARTLAR GİZLENDİ");
        }

        yield return StartCoroutine(BattleUIBroadcast.Instance.ShowMessageRoutine("DÜŞMANIN SIRASI", 1.5f));

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.OnTurnStart();
            yield return new WaitForSeconds(2f);
            enemy.ExecuteAction(Player.Instance);
            enemy.OnTurnEnd();
        }

        yield return StartCoroutine(BattleUIBroadcast.Instance.ShowMessageRoutine("SIRA SENDE!", 1.5f));

        if (handObject != null)
        {
            handObject.SetActive(true);
            Debug.Log("KARTLAR GERİ GELDİ");
        }

        Player.Instance?.OnTurnStart();
        isProcessingTurn = false;
    }

    private void OnDestroy()
    {
        if (DeckManager.Instance != null)
            DeckManager.Instance.OnTurnEnded -= OnPlayerTurnEnded;
    }
}