using System.Collections.Generic;
using UnityEngine;

public class EnemyHUDManager : MonoBehaviour
{
    [Tooltip("EnemyPanel prefab. One instance is created per living enemy.")]
    [SerializeField] private EnemyPanel panelPrefab;

    [Tooltip("Parent RectTransform inside GameCanvas where panels are placed.")]
    [SerializeField] private RectTransform panelContainer;

    private readonly Dictionary<Enemy, EnemyPanel> panels = new Dictionary<Enemy, EnemyPanel>();

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null || panelPrefab == null || panelContainer == null) return;

        EnemyPanel panel = Instantiate(panelPrefab, panelContainer);
        panel.Track(enemy);
        panels[enemy] = panel;

        enemy.OnDied += () => panels.Remove(enemy);
    }

    public void ClearAll()
    {
        foreach (EnemyPanel panel in panels.Values)
            if (panel != null) Destroy(panel.gameObject);

        panels.Clear();
    }
}
