public interface IStatusEffect
{
    string Id { get; }

    int Stacks { get; }

    void Stack(int amount);

    void OnTurnStart(ICombatEntity entity);

    void OnTurnEnd(ICombatEntity entity);

    bool IsExpired { get; }
  
    public virtual int ModifyOutgoingDamage(int damage) => damage;
    public virtual int ModifyIncomingDamage(int damage) => damage;
}
