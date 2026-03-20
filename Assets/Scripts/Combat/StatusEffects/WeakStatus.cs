/// <summary>
/// Weak: entity deals 25% less damage. Decrements by 1 at end of turn.
/// Applied to the attacker; checked in damage resolution.
/// </summary>
public class WeakStatus : StatusEffectBase
{
    public const string StatusId = "Weak";
    public override string Id => StatusId;

    /// <summary>Outgoing damage multiplier while Weak is active.</summary>
    public const float DamageMultiplier = 0.75f;

    public WeakStatus(int stacks) => Stacks = stacks;

    public override void OnTurnEnd(ICombatEntity entity) => Stacks--;
}
