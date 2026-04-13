using System.Collections.Generic;
using UnityEngine;

public static class CardEffectResolver
{
    private static int globalEnemyDamageTakenBonus = 0;
    private static int globalEnemyDamageDealtReduction = 0;

    public static int GlobalEnemyDamageTakenBonus => globalEnemyDamageTakenBonus;

    public static int GlobalEnemyDamageDealtReduction => globalEnemyDamageDealtReduction;

    public static void ResetCombatModifiers()
    {
        globalEnemyDamageTakenBonus = 0;
        globalEnemyDamageDealtReduction = 0;
    }

    public static void Resolve(CardData cardData, Collider2D singleTarget)
    {
        Player player = Player.Instance;

        foreach (CardEffect effect in cardData.effects)
        {
            switch (cardData.targetType)
            {
                case TargetType.SingleEnemy:
                    if (singleTarget != null)
                    {
                        Enemy enemy = singleTarget.GetComponent<Enemy>();
                        if (enemy != null)
                            ApplyToEnemy(effect, enemy, player);
                    }
                    break;

                case TargetType.AllEnemies:

                    if (BattleManager.Instance != null)
                    {

                        var enemies = new List<Enemy>(BattleManager.Instance.ActiveEnemies);
                        foreach (Enemy enemy in enemies)
                        {
                            if (enemy != null)
                                ApplyToEnemy(effect, enemy, player);
                        }
                    }
                    break;

                case TargetType.Self:
                case TargetType.None:
                    ApplyToPlayer(effect, player);
                    break;
            }
        }
    }

    private static void ApplyToEnemy(CardEffect effect, Enemy enemy, Player player)
    {
        if (enemy == null) return;

        switch (effect.type)
        {
            case CardEffectType.Damage:
                int thorns = enemy.GetThorns();

                int rawDamage = player != null
                    ? player.CalculateOutgoingDamage(effect.value)
                    : effect.value;
                int finalDamage = rawDamage + globalEnemyDamageTakenBonus;
                enemy.TakeDamage(finalDamage);

                if (thorns > 0 && player != null)
                    player.TakeDamage(thorns);
                break;

            case CardEffectType.Block:
                enemy.GainShield(effect.value);
                break;

            case CardEffectType.ApplyPoison:
                enemy.ApplyStatus(new PoisonStatus(effect.value));
                break;

            case CardEffectType.ApplyBurn:
                enemy.ApplyStatus(new BurnStatus(effect.value));
                break;

            case CardEffectType.ApplyVulnerable:
                enemy.ApplyStatus(new VulnerableStatus(effect.value));
                break;

            case CardEffectType.ApplyWeak:
                enemy.ApplyStatus(new WeakStatus(effect.value));
                break;

            case CardEffectType.ApplyFrail:
                enemy.ApplyStatus(new FrailStatus(effect.value));
                break;

            case CardEffectType.Heal:
                enemy.Heal(effect.value);
                break;
        }
    }

    private static void ApplyToPlayer(CardEffect effect, Player player)
    {
        if (player == null) return;

        switch (effect.type)
        {
            case CardEffectType.Block:
                player.GainShield(effect.value);
                break;

            case CardEffectType.PermanentBlock:
                player.GainPermanentShield(effect.value);
                break;

            case CardEffectType.Heal:
                player.Heal(effect.value);
                break;

            case CardEffectType.SelfDamage:
                player.TakeDamage(effect.value);
                break;

            case CardEffectType.DrawCard:
                DeckManager.Instance?.DrawCards(effect.value);
                break;

            case CardEffectType.DiscardCard:
                DiscardRandomCards(effect.value);
                break;

            case CardEffectType.SetHandSize:
                DeckManager.Instance?.SetHandSize(effect.value);
                break;

            case CardEffectType.ReduceCardCost:
                ReduceRandomCardCostInHand(effect.value);
                break;

            case CardEffectType.GainStrength:
                player.ApplyStatus(new StrengthStatus(effect.value));
                break;

            case CardEffectType.GainRegeneration:
                player.ApplyStatus(new RegenerationStatus(effect.value));
                break;

            case CardEffectType.GainThorns:
                player.ApplyStatus(new ThornsStatus(effect.value));
                break;

            case CardEffectType.ApplyPoison:
                player.ApplyStatus(new PoisonStatus(effect.value));
                break;

            case CardEffectType.ApplyBurn:
                player.ApplyStatus(new BurnStatus(effect.value));
                break;

            case CardEffectType.ApplyVulnerable:
                player.ApplyStatus(new VulnerableStatus(effect.value));
                break;

            case CardEffectType.ApplyWeak:
                player.ApplyStatus(new WeakStatus(effect.value));
                break;

            case CardEffectType.ApplyFrail:
                player.ApplyStatus(new FrailStatus(effect.value));
                break;

            case CardEffectType.AmplifiyEnemyDamageTaken:
                globalEnemyDamageTakenBonus += effect.value;
                break;

            case CardEffectType.ReduceEnemyDamageDealt:
                globalEnemyDamageDealtReduction += effect.value;
                break;
        }
    }
    private static void DiscardRandomCards(int count)
    {
        DeckManager deck = DeckManager.Instance;
        if (deck == null) return;

        List<Card> handSnapshot = new List<Card>(deck.Hand);
        int toDiscard = Mathf.Min(count, handSnapshot.Count);

        for (int i = 0; i < toDiscard; i++)
        {
            int randomIndex = Random.Range(0, handSnapshot.Count);
            Card card = handSnapshot[randomIndex];
            handSnapshot.RemoveAt(randomIndex);
            deck.DiscardCard(card);
        }
    }

    private static void ReduceRandomCardCostInHand(int reduction)
    {
        DeckManager deck = DeckManager.Instance;
        if (deck == null || deck.HandCount == 0) return;

        List<Card> handSnapshot = new List<Card>(deck.Hand);
        int randomIndex = Random.Range(0, handSnapshot.Count);
        handSnapshot[randomIndex].ReduceManaCost(reduction);
    }
}

