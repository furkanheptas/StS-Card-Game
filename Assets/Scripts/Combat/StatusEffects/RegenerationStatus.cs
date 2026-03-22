public class RegenerationStatus : StatusEffectBase
{
    public const string StatusId = "Regeneration";
    public override string Id => StatusId;

    public RegenerationStatus(int stacks) => Stacks = stacks;

    public override void OnTurnStart(ICombatEntity entity)
    {
        entity.Heal(Stacks);
        Stacks--;
    }
}
