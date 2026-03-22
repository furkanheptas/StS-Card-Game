public class FrailStatus : StatusEffectBase
{
    public const string StatusId = "Frail";
    public override string Id => StatusId;

    public const float BlockMultiplier = 0.75f;

    public FrailStatus(int stacks) => Stacks = stacks;

    public override void OnTurnEnd(ICombatEntity entity) => Stacks--;
}
