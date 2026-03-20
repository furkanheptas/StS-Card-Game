using System.Collections.Generic;

public interface ICombatEntity
{
    int CurrentHP    { get; }
    int CurrentShield { get; }

    void TakeDamage(int amount);
    void GainShield(int amount);
    void Heal(int amount);

    float OutgoingDamageMultiplier { get; }

    int Strength { get; }

    IReadOnlyList<IStatusEffect> StatusEffects { get; }

    void ApplyStatus(IStatusEffect effect);

    void RemoveStatus(string id);
}
