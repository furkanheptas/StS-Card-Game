/// <summary>
/// Regeneration: heals (stacks) HP at the start of the entity's turn, then decrements by 1.
/// </summary>
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
