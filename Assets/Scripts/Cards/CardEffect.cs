using System;
using UnityEngine;

public enum CardEffectType { Damage, Block, Heal, DrawCard }

[Serializable]
public class CardEffect
{
    [Tooltip("What the effect does.")]
    public CardEffectType type;

    [Tooltip("Value of the damage dealt, block gained, HP healed, or cards drawn.")]
    public int value;
}
