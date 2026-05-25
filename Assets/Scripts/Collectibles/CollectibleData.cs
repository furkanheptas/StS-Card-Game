using UnityEngine;

public enum CollectibleRarity { Common, Uncommon, Rare, Boss, Starter }

[CreateAssetMenu(fileName = "NewCollectible", menuName = "Game/Collectible Data")]
public class CollectibleData : ScriptableObject
{
    [Header("Identity")]
    public string collectibleName;
    [TextArea] public string description;
    public Sprite artwork;
    public CollectibleRarity rarity;

    [Header("Passive Effects")]
    [Tooltip("Flat HP bonus applied when picked up.")]
    public int bonusMaxHP;

    [Tooltip("Flat starting mana bonus each combat.")]
    public int bonusMana;

    [Tooltip("Extra cards drawn at the start of each turn.")]
    public int bonusCardDraw;

    [Tooltip("Flat Strength bonus applied at combat start.")]
    public int bonusStrength;

    [Tooltip("Starting gold bonus (run start only).")]
    public int bonusStartingGold;
}
