public class ThornsStatus : StatusEffectBase
{
    public const string StatusId = "Thorns";
    public override string Id => StatusId;

    public ThornsStatus(int stacks) => Stacks = stacks;

    public override bool IsExpired => false;
}
