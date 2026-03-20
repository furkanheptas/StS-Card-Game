/// <summary>
/// Contract for all turn-based status effects applied to a combat entity.
/// </summary>
public interface IStatusEffect
{
    /// <summary>Unique identifier used for stacking and UI lookup.</summary>
    string Id { get; }

    /// <summary>Current stack / duration count displayed on the icon.</summary>
    int Stacks { get; }

    /// <summary>Add stacks to this effect.</summary>
    void Stack(int amount);

    /// <summary>Called at the start of the entity's turn.</summary>
    void OnTurnStart(ICombatEntity entity);

    /// <summary>Called at the end of the entity's turn.</summary>
    void OnTurnEnd(ICombatEntity entity);

    /// <summary>Returns true when the effect should be removed.</summary>
    bool IsExpired { get; }
}
