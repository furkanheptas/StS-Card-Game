/// <summary>
/// Thorns: reflects a fixed amount of damage back to any attacker.
/// Permanent; checked in damage resolution when an attack lands.
/// </summary>
public class ThornsStatus : StatusEffectBase
{
    public const string StatusId = "Thorns";
    public override string Id => StatusId;

    public ThornsStatus(int stacks) => Stacks = stacks;

    public override bool IsExpired => false;
}
