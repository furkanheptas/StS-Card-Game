using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string enemyName;
    public Sprite sprite;

    [Header("Stats")]
    public int maxHP;

    [Header("Action Pattern")]
    [Tooltip("Actions cycle of the enemy.")]
    public List<EnemyActionData> actions = new List<EnemyActionData>();
}
