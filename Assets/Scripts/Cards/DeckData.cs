using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeckData", menuName = "Cards/Deck Data")]
public class DeckData : ScriptableObject
{
    [Header("Starting Deck")]
    public List<CardData> cards = new List<CardData>();
}
