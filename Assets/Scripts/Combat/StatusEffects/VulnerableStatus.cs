/// <summary>
/// Vulnerable: entity takes 50% more damage. Decrements by 1 at end of turn.
/// The multiplier is read by damage-dealing code via ICombatEntity.
/// </summary>
public class VulnerableStatus : StatusEffectBase
{
    public const string StatusId = "Vulnerable";
    public override string Id => StatusId;

    /// <summary>Extra damage multiplier applied to incoming hits.</summary>
    public const float DamageMultiplier = 1.5f;

    public VulnerableStatus(int stacks) => Stacks = stacks;

    public override void OnTurnEnd(ICombatEntity entity) => Stacks--;
}
