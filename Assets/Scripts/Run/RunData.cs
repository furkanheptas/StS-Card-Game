using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunData
{
    public CharacterData selectedCharacter;

    public int currentHP;
    public int maxHP;
    public int gold;
    public List<CardData> deck = new List<CardData>();
    public List<CollectibleData> collectibles = new List<CollectibleData>();
    public int currentFloor;
    public int currentNodeIndex;
    public List<MapNodeData> mapNodes = new List<MapNodeData>();
    public int totalGoldEarned;
    public int enemiesDefeated;
    public int floorsCleared;

    public void Initialize(CharacterData character)
    {
        selectedCharacter = character;
        maxHP = character.startingMaxHP;
        currentHP = maxHP;
        gold = character.startingGold;
        currentFloor = 0;
        currentNodeIndex = -1;
        totalGoldEarned = 0;
        enemiesDefeated = 0;
        floorsCleared = 0;

        deck = new List<CardData>(character.startingDeck);

        collectibles = new List<CollectibleData>();
        foreach (CollectibleData c in character.startingCollectibles)
            AddCollectible(c);
    }
    public void AddCollectible(CollectibleData collectible)
    {
        collectibles.Add(collectible);
        maxHP += collectible.bonusMaxHP;
        if (collectible.bonusMaxHP > 0)
            currentHP += collectible.bonusMaxHP;
        gold += collectible.bonusStartingGold;
    }

    public void AddCard(CardData card) => deck.Add(card);

    public bool RemoveCard(CardData card) => deck.Remove(card);
    public bool UpgradeCard(CardData card)
    {
        if (card == null || !card.CanUpgrade) return false;
        int idx = deck.IndexOf(card);
        if (idx < 0) return false;
        deck[idx] = card.upgradedVersion;
        return true;
    }

    public void AddGold(int amount)
    {
        gold = Mathf.Max(0, gold + amount);
        totalGoldEarned += Mathf.Max(0, amount);
    }

    public void SpendGold(int amount) => gold = Mathf.Max(0, gold - amount);

    public void HealHP(int amount) => currentHP = Mathf.Min(maxHP, currentHP + amount);

    public void TakeDamage(int amount) => currentHP = Mathf.Max(0, currentHP - amount);

    public int GetBonusMana()
    {
        int total = 0;
        foreach (CollectibleData c in collectibles) total += c.bonusMana;
        return total;
    }

    public int GetBonusCardDraw()
    {
        int total = 0;
        foreach (CollectibleData c in collectibles) total += c.bonusCardDraw;
        return total;
    }

    public int GetBonusStrength()
    {
        int total = 0;
        foreach (CollectibleData c in collectibles) total += c.bonusStrength;
        return total;
    }
}
