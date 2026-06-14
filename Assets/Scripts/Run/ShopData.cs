using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopData", menuName = "Game/Shop Data")]
public class ShopData : ScriptableObject
{
    [Tooltip("Display name for this shop shown in the editor list.")]
    public string shopName;

    [Header("Cards for Sale")]
    public List<CardData> cardPool = new List<CardData>();
    [Tooltip("How many card slots the shop shows.")]
    public int cardSlotCount = 5;
    public int cardPriceMin = 50;
    public int cardPriceMax = 150;

    [Header("Collectibles for Sale")]
    public List<CollectibleData> collectiblePool = new List<CollectibleData>();
    [Tooltip("How many collectible slots the shop shows.")]
    public int collectibleSlotCount = 2;
    public int collectiblePriceMin = 150;
    public int collectiblePriceMax = 300;

    [Header("Card Removal")]
    [Tooltip("Gold cost to remove one card from the deck.")]
    public int removalCost = 75;

    [Header("Card Upgrade")]
    [Tooltip("Gold cost to upgrade one card at the shop.")]
    public int upgradeCost = 90;
}
