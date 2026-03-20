/// <summary>
/// Frail: entity gains 25% less Block. Decrements by 1 at end of turn.
/// Checked in block-gaining code.
/// </summary>
public class FrailStatus : StatusEffectBase
{
    public const string StatusId = "Frail";
    public override string Id => StatusId;

    /// <summary>Multiplier applied to Block gained while Frail is active.</summary>
    public const float BlockMultiplier = 0.75f;

    public FrailStatus(int stacks) => Stacks = stacks;

    public override void OnTurnEnd(ICombatEntity entity) => Stacks--;
}
