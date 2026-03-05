using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "Game/Encounter Data")]
public class EncounterData : ScriptableObject
{
    public List<EncounterSlot> slots = new List<EncounterSlot>();
}

[Serializable]
public class EncounterSlot
{
    [Tooltip("The enemy type to spawn.")]
    public EnemyData enemyData;

    [Tooltip("Position offset from the encounter's spawn anchor.")]
    public Vector2 positionOffset;
}
