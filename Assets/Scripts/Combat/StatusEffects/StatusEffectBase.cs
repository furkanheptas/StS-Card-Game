/// <summary>
/// Convenience base class with default no-op turn hooks.
/// Concrete effects only need to override what they use.
/// </summary>
public abstract class StatusEffectBase : IStatusEffect
{
    public abstract string Id     { get; }
    public          int    Stacks { get; protected set; }
    public virtual  bool   IsExpired => Stacks <= 0;

    public virtual void Stack(int amount) => Stacks += amount;
    public virtual void OnTurnStart(ICombatEntity entity) { }
    public virtual void OnTurnEnd(ICombatEntity entity)   { }
}
