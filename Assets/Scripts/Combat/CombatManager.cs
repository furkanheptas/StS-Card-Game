using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private void Start()
    {
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
        Player.Instance?.OnTurnEnd();

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
            enemy.ExecuteAction(Player.Instance);
    }
}
