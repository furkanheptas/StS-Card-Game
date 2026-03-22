public class WeakStatus : StatusEffectBase
{
    public const string StatusId = "Weak";
    public override string Id => StatusId;

    public const float DamageMultiplier = 0.75f;

    public WeakStatus(int stacks) => Stacks = stacks;

    public override void OnTurnEnd(ICombatEntity entity) => Stacks--;
}
