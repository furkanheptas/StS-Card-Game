using System;
using UnityEngine;

public enum CardEffectType
{
    Damage,

    Block,

    PermanentBlock,

    Heal,

    SelfDamage,

    DrawCard,

    DiscardCard,

    SetHandSize,

    ReduceCardCost,

    GainStrength,

    GainRegeneration,

    GainThorns,

    ApplyPoison,

    ApplyBurn,

    ApplyVulnerable,

    ApplyWeak,

    ApplyFrail,

    AmplifiyEnemyDamageTaken,

    ReduceEnemyDamageDealt,
}

[Serializable]
public class CardEffect
{
    [Tooltip("What the effect does.")]
    public CardEffectType type;

    [Tooltip("Primary numeric value (damage, stacks, card count, etc.).")]
    public int value;
}
