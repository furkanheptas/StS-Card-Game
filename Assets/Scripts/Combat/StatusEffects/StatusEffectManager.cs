using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager
{
    private readonly List<IStatusEffect> effects = new List<IStatusEffect>();

    public IReadOnlyList<IStatusEffect> Effects => effects;

    public void Apply(IStatusEffect incoming)
    {
        IStatusEffect existing = effects.Find(e => e.Id == incoming.Id);
        if (existing != null)
            existing.Stack(incoming.Stacks);
        else
            effects.Add(incoming);
    }

    public void Remove(string id) => effects.RemoveAll(e => e.Id == id);

    public IStatusEffect Get(string id) => effects.Find(e => e.Id == id);

    public bool Has(string id) => Get(id) != null && Get(id).Stacks > 0;

    public void TickTurnStart(ICombatEntity entity)
    {
        foreach (IStatusEffect effect in new List<IStatusEffect>(effects))
            effect.OnTurnStart(entity);

        Prune();
    }

    public void TickTurnEnd(ICombatEntity entity)
    {
        foreach (IStatusEffect effect in new List<IStatusEffect>(effects))
            effect.OnTurnEnd(entity);

        Prune();
    }

    private void Prune() => effects.RemoveAll(e => e.IsExpired);

    public int GetStrength()
    {
        IStatusEffect s = Get(StrengthStatus.StatusId);
        return s != null ? s.Stacks : 0;
    }

    public int CalculateOutgoingDamage(int baseDamage)
    {
        float damage = baseDamage + GetStrength();

        if (Has(WeakStatus.StatusId))
            damage *= WeakStatus.DamageMultiplier;

        return Mathf.Max(0, Mathf.RoundToInt(damage));
    }

    public int CalculateIncomingDamage(int baseDamage)
    {
        float damage = baseDamage;

        if (Has(VulnerableStatus.StatusId))
            damage *= VulnerableStatus.DamageMultiplier;

        return Mathf.RoundToInt(damage);
    }

    public int CalculateBlockGain(int baseBlock)
    {
        float block = baseBlock;

        if (Has(FrailStatus.StatusId))
            block *= FrailStatus.BlockMultiplier;

        return Mathf.Max(0, Mathf.RoundToInt(block));
    }

    public int GetThorns()
    {
        IStatusEffect t = Get(ThornsStatus.StatusId);
        return t != null ? t.Stacks : 0;
    }
}
