public class PoisonStatus : StatusEffectBase
{
    public const string StatusId = "Poison";
    public override string Id => StatusId;

    public PoisonStatus(int stacks) => Stacks = stacks;

    public override void OnTurnStart(ICombatEntity entity)
    {
        entity.TakeDamage(Stacks);
        Stacks--;
    }
}
