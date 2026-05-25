using System.Collections.Generic;
using UnityEngine;

public enum CardType { Attack, Skill, Power }
public enum TargetType { SingleEnemy, AllEnemies, Self, None }

[CreateAssetMenu(fileName = "NewCardData", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Identity")]
    public string cardName;
    [TextArea] public string description;
    public Sprite artwork;

    [Header("Stats")]
    public int manaCost;
    public CardType cardType;
    public TargetType targetType;

    [Header("Effects")]
    public List<CardEffect> effects = new List<CardEffect>();

    [Header("Upgrade")]
    [Tooltip("The upgraded version of this card. If null, this card cannot be upgraded.")]
    public CardData upgradedVersion;
    public bool CanUpgrade => upgradedVersion != null;
}
