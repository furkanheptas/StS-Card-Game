public abstract class StatusEffectBase : IStatusEffect
{
    public abstract string Id     { get; }
    public          int    Stacks { get; protected set; }
    public virtual  bool   IsExpired => Stacks <= 0;

    public virtual void Stack(int amount) => Stacks += amount;
    public virtual void OnTurnStart(ICombatEntity entity) { }
    public virtual void OnTurnEnd(ICombatEntity entity)   { }
    public virtual int ModifyOutgoingDamage(int damage) => damage;
    public virtual int ModifyIncomingDamage(int damage) => damage;
}
