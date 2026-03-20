/// <summary>
/// Strength: flat bonus added to every attack. Permanent (does not decrement).
/// Stacks add together. Negative stacks reduce damage.
/// </summary>
public class StrengthStatus : StatusEffectBase
{
    public const string StatusId = "Strength";
    public override string Id => StatusId;

    public StrengthStatus(int stacks) => Stacks = stacks;

    // Strength does not expire on its own.
    public override bool IsExpired => false;
}
