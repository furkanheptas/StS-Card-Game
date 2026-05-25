using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventEffectType
{
    GainGold,
    LoseGold,
    GainHP,
    LoseHP,
    GainCard,
    GainCollectible,
    RemoveCard,
}

[Serializable]
public class EventChoice
{
    public string choiceText;
    [TextArea] public string resultDescription;

    [Header("Effects")]
    public EventEffectType effectType;
    public int value;
    public CardData cardReward;
    public CollectibleData collectibleReward;
}

[CreateAssetMenu(fileName = "NewEvent", menuName = "Game/Event Data")]
public class EventData : ScriptableObject
{
    [Header("Presentation")]
    public string eventName;
    [TextArea(3, 6)] public string description;
    public Sprite artwork;

    [Header("Choices")]
    public List<EventChoice> choices = new List<EventChoice>();
}
