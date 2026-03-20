/// <summary>
/// Burn: deals a fixed amount of damage at the end of the entity's turn, then decrements by 1.
/// </summary>
public class BurnStatus : StatusEffectBase
{
    public const string StatusId = "Burn";
    public override string Id => StatusId;

    private readonly int damagePerStack;

    public BurnStatus(int stacks, int damagePerStack = 1)
    {
        Stacks = stacks;
        this.damagePerStack = damagePerStack;
    }

    public override void OnTurnEnd(ICombatEntity entity)
    {
        entity.TakeDamage(Stacks * damagePerStack);
        Stacks--;
    }
}
