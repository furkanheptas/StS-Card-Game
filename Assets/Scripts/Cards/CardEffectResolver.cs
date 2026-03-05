using UnityEngine;

public static class CardEffectResolver
{
    public static void Resolve(CardData cardData, Collider2D singleTarget)
    {
        switch (cardData.targetType)
        {
            case TargetType.SingleEnemy:
                if (singleTarget != null)
                {
                    Enemy enemy = singleTarget.GetComponent<Enemy>();
                    foreach (CardEffect effect in cardData.effects)
                        ApplyToEnemy(effect, enemy);
                }
                break;

            case TargetType.AllEnemies:
                foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
                    foreach (CardEffect effect in cardData.effects)
                        ApplyToEnemy(effect, enemy);
                break;

            case TargetType.Self:
            case TargetType.None:
                foreach (CardEffect effect in cardData.effects)
                    ApplyToPlayer(effect);
                break;
        }
    }

    private static void ApplyToEnemy(CardEffect effect, Enemy enemy)
    {
        if (enemy == null) return;

        switch (effect.type)
        {
            case CardEffectType.Damage: enemy.TakeDamage(effect.value); break;
            case CardEffectType.Block:  enemy.GainShield(effect.value); break;
        }
    }

    private static void ApplyToPlayer(CardEffect effect)
    {
        Player player = Player.Instance;
        if (player == null) return;

        switch (effect.type)
        {
            case CardEffectType.Block:    player.GainShield(effect.value);              break;
            case CardEffectType.Heal:     player.Heal(effect.value);                    break;
            case CardEffectType.DrawCard: DeckManager.Instance?.DrawCards(effect.value); break;
        }
    }
}
