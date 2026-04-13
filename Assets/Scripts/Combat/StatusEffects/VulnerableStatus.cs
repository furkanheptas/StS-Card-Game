using UnityEngine;

public class VulnerableStatus : StatusEffectBase
{
    public const string StatusId = "Vulnerable";
    public override string Id => StatusId;

    public const float DamageMultiplier = 1.5f;

    public VulnerableStatus(int stacks) => Stacks = stacks;

    public override void OnTurnEnd(ICombatEntity entity) => Stacks--;
    public override int ModifyIncomingDamage(int damage) => Mathf.RoundToInt(damage * DamageMultiplier);
}
