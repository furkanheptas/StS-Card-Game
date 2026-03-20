using System.Collections.Generic;

/// <summary>
/// Common interface implemented by Player and Enemy so status effects
/// can interact with any combat entity without casting.
/// </summary>
public interface ICombatEntity
{
    int CurrentHP    { get; }
    int CurrentShield { get; }

    void TakeDamage(int amount);
    void GainShield(int amount);
    void Heal(int amount);

    /// <summary>Multiplier applied to all outgoing damage. Default = 1.</summary>
    float OutgoingDamageMultiplier { get; }

    /// <summary>Flat bonus added to all outgoing damage (Strength).</summary>
    int Strength { get; }

    /// <summary>Read-only view of active status effects.</summary>
    IReadOnlyList<IStatusEffect> StatusEffects { get; }

    /// <summary>Apply or stack a status effect.</summary>
    void ApplyStatus(IStatusEffect effect);

    /// <summary>Remove a specific status effect by id.</summary>
    void RemoveStatus(string id);
}
