using System;
using UnityEngine;

public enum EnemyActionType { Attack, Defend, Buff, Debuff }

[Serializable]
public class EnemyActionData
{
    public EnemyActionType type;

    [Tooltip("Damage dealt or shield gained.")]
    public int value;

    [Tooltip("Icon shown above the enemy.")]
    public Sprite intentIcon;

    [Tooltip("Optional label shown next to iocn.")]
    public string label;
}
