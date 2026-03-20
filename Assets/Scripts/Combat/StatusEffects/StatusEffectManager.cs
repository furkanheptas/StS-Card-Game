using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a list of status effects for a single combat entity.
/// Handles stacking, tick ordering, and cleanup of expired effects.
/// </summary>
public class StatusEffectManager
{
    private readonly List<IStatusEffect> effects = new List<IStatusEffect>();

    public IReadOnlyList<IStatusEffect> Effects => effects;

    /// <summary>Apply or stack an effect. If an effect with the same Id already
    /// exists, its Stack() method is called instead of adding a duplicate.</summary>
    public void Apply(IStatusEffect incoming)
    {
        IStatusEffect existing = effects.Find(e => e.Id == incoming.Id);
        if (existing != null)
            existing.Stack(incoming.Stacks);
        else
            effects.Add(incoming);
    }

    /// <summary>Remove an effect by its Id.</summary>
    public void Remove(string id) => effects.RemoveAll(e => e.Id == id);

    /// <summary>Returns the first effect matching id, or null.</summary>
    public IStatusEffect Get(string id) => effects.Find(e => e.Id == id);

    /// <summary>Returns true if the entity has at least one stack of the given effect.</summary>
    public bool Has(string id) => Get(id) != null && Get(id).Stacks > 0;

    /// <summary>Tick all effects at the start of the entity's turn and prune expired ones.</summary>
    public void TickTurnStart(ICombatEntity entity)
    {
        foreach (IStatusEffect effect in new List<IStatusEffect>(effects))
            effect.OnTurnStart(entity);

        Prune();
    }

    /// <summary>Tick all effects at the end of the entity's turn and prune expired ones.</summary>
    public void TickTurnEnd(ICombatEntity entity)
    {
        foreach (IStatusEffect effect in new List<IStatusEffect>(effects))
            effect.OnTurnEnd(entity);

        Prune();
    }

    private void Prune() => effects.RemoveAll(e => e.IsExpired);

    // ---- Strength helper --------------------------------------------------

    /// <summary>Total flat strength bonus (can be negative).</summary>
    public int GetStrength()
    {
        IStatusEffect s = Get(StrengthStatus.StatusId);
        return s != null ? s.Stacks : 0;
    }

    // ---- Damage calculation helpers ----------------------------------------

    /// <summary>
    /// Calculates final outgoing damage after applying Strength and Weak.
    /// </summary>
    public int CalculateOutgoingDamage(int baseDamage)
    {
        float damage = baseDamage + GetStrength();

        if (Has(WeakStatus.StatusId))
            damage *= WeakStatus.DamageMultiplier;

        return Mathf.Max(0, Mathf.RoundToInt(damage));
    }

    /// <summary>
    /// Calculates final incoming damage after applying Vulnerable.
    /// </summary>
    public int CalculateIncomingDamage(int baseDamage)
    {
        float damage = baseDamage;

        if (Has(VulnerableStatus.StatusId))
            damage *= VulnerableStatus.DamageMultiplier;

        return Mathf.RoundToInt(damage);
    }

    /// <summary>
    /// Calculates final block amount after applying Frail.
    /// </summary>
    public int CalculateBlockGain(int baseBlock)
    {
        float block = baseBlock;

        if (Has(FrailStatus.StatusId))
            block *= FrailStatus.BlockMultiplier;

        return Mathf.Max(0, Mathf.RoundToInt(block));
    }

    /// <summary>Thorns stacks on this entity (0 if none).</summary>
    public int GetThorns()
    {
        IStatusEffect t = Get(ThornsStatus.StatusId);
        return t != null ? t.Stacks : 0;
    }
}
