/// <summary>
/// Poison: deals (stacks) damage at the start of the entity's turn, then decrements by 1.
/// Mirrors Slay the Spire behaviour exactly.
/// </summary>
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
