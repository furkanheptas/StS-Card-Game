using System;
using UnityEngine;

public enum CardEffectType
{
    // ---- Direct -------------------------------------------------------
    /// <summary>Deal damage to the target (modified by Strength / Weak / Vulnerable).</summary>
    Damage,

    /// <summary>Give the player shield that resets at turn end.</summary>
    Block,

    /// <summary>Give the player shield that does NOT reset at turn end.</summary>
    PermanentBlock,

    /// <summary>Restore HP to the player.</summary>
    Heal,

    /// <summary>Deal damage to self (no shield absorption).</summary>
    SelfDamage,

    // ---- Deck manipulation --------------------------------------------
    /// <summary>Draw cards from the draw pile.</summary>
    DrawCard,

    /// <summary>Discard a number of cards at random from the player's hand.</summary>
    DiscardCard,

    /// <summary>Set how many cards the player starts each turn with for this combat.</summary>
    SetHandSize,

    /// <summary>Reduce the mana cost of a random card in hand by value (min 0).</summary>
    ReduceCardCost,

    // ---- Stat buffs ---------------------------------------------------
    /// <summary>Grant the player Strength (flat damage bonus per attack).</summary>
    GainStrength,

    /// <summary>Grant the player Regeneration (heals stacks HP per turn, then decrements).</summary>
    GainRegeneration,

    /// <summary>Grant the player Thorns (reflects damage on hit).</summary>
    GainThorns,

    // ---- Debuffs on enemies -------------------------------------------
    /// <summary>Apply Poison stacks to target (deals stacks damage per turn, decrements).</summary>
    ApplyPoison,

    /// <summary>Apply Burn stacks to target (deals damage at turn end, decrements).</summary>
    ApplyBurn,

    /// <summary>Apply Vulnerable to target (takes 50% more damage).</summary>
    ApplyVulnerable,

    /// <summary>Apply Weak to target (deals 25% less damage).</summary>
    ApplyWeak,

    /// <summary>Apply Frail to target (gains 25% less Block).</summary>
    ApplyFrail,

    // ---- Amplifiers ---------------------------------------------------
    /// <summary>Increase all damage enemies take this combat by a flat amount.</summary>
    AmplifiyEnemyDamageTaken,

    /// <summary>Reduce all damage enemies deal this combat by a flat amount.</summary>
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
