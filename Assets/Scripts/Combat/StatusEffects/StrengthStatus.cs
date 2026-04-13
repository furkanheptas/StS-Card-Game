public class StrengthStatus : StatusEffectBase
{
    public const string StatusId = "Strength";
    public override string Id => StatusId;

    public StrengthStatus(int stacks) => Stacks = stacks;

    public override bool IsExpired => false;
    public override int ModifyOutgoingDamage(int damage) => damage + Stacks;
}
