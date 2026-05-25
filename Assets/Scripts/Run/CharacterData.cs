using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public string characterName;
    [TextArea] public string characterDescription;
    public Sprite portrait;
    public Sprite combatSprite;

    [Header("Starting Stats")]
    public int startingMaxHP = 80;
    public int startingGold = 100;
    public int startingMana = 3;

    [Header("Starting Deck")]
    public List<CardData> startingDeck = new List<CardData>();

    [Header("Starting Collectibles")]
    public List<CollectibleData> startingCollectibles = new List<CollectibleData>();
}
