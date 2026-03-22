public interface IStatusEffect
{
    string Id { get; }

    int Stacks { get; }

    void Stack(int amount);

    void OnTurnStart(ICombatEntity entity);

    void OnTurnEnd(ICombatEntity entity);

    bool IsExpired { get; }
}
