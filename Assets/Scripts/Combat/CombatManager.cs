using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private void Start()
    {
        CardEffectResolver.ResetCombatModifiers();

        if (DeckManager.Instance != null)
            DeckManager.Instance.OnTurnEnded += OnPlayerTurnEnded;
    }

    private void OnDestroy()
    {
        if (DeckManager.Instance != null)
            DeckManager.Instance.OnTurnEnded -= OnPlayerTurnEnded;
    }

    private void OnPlayerTurnEnded()
    {
        // Player turn-end status ticks
        Player.Instance?.OnTurnEnd();

        // Enemy turn: tick start → execute action → tick end
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            enemy.OnTurnStart();
            enemy.ExecuteAction(Player.Instance);
            enemy.OnTurnEnd();
        }

        // Player turn-start status ticks for next turn
        Player.Instance?.OnTurnStart();
    }
}
