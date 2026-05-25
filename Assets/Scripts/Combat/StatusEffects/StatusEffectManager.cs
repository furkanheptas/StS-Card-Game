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

    public int GetStrength() => Has(StrengthStatus.StatusId) ? Get(StrengthStatus.StatusId).Stacks : 0;
    public int GetThorns() => Has(ThornsStatus.StatusId) ? Get(ThornsStatus.StatusId).Stacks : 0;

    public int CalculateOutgoingDamage(int baseDamage)
    {
        int modifiedDamage = baseDamage;
        foreach (var effect in effects)
            modifiedDamage = effect.ModifyOutgoingDamage(modifiedDamage);

        return Mathf.Max(0, modifiedDamage);
    }

    public int CalculateIncomingDamage(int baseDamage)
    {
        int modifiedDamage = baseDamage;
        foreach (IStatusEffect effect in effects)
            modifiedDamage = effect.ModifyIncomingDamage(modifiedDamage);

        return Mathf.Max(0, modifiedDamage);
    }

    public int CalculateBlockGain(int baseBlock)
    {
        int modifiedBlock = baseBlock;
       
        if (Has(FrailStatus.StatusId))
            modifiedBlock = Mathf.RoundToInt(modifiedBlock * FrailStatus.BlockMultiplier);

        return Mathf.Max(0, modifiedBlock);
    }
}