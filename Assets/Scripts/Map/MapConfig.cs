using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapConfig", menuName = "Game/Map Config")]
public class MapConfig : ScriptableObject
{
    [Header("Layout")]
    [Tooltip("Number of floors (rows) before the boss floor.")]
    public int floorCount = 15;

    [Tooltip("Number of columns (paths) per floor.")]
    public int columnsPerFloor = 7;

    [Header("Node Type Weights (Non-Boss Floors)")]
    public float weightNormal = 45f;
    public float weightElite  = 15f;
    public float weightShop   = 10f;
    public float weightEvent  = 20f;
    public float weightRest   = 10f;

    [Header("Elite Floors")]
    [Tooltip("Floors on which Elite nodes are guaranteed (1-indexed).")]
    public List<int> eliteGuaranteedFloors = new List<int> { 6, 11 };

    [Header("Boss")]
    [Tooltip("The single boss encounter used at the last floor.")]
    public EncounterData bossEncounter;

    [Header("Normal Encounter Pool")]
    public List<EncounterData> normalEncounters = new List<EncounterData>();

    [Header("Elite Encounter Pool")]
    public List<EncounterData> eliteEncounters = new List<EncounterData>();

    [Header("Event Pool")]
    public List<EventData> events = new List<EventData>();
}
