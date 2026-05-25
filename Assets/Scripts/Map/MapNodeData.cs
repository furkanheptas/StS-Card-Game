using System;
using System.Collections.Generic;
using UnityEngine;

public enum MapNodeType { Normal, Elite, Shop, Event, Boss, Rest }

/// <summary>
/// Runtime data for a single node on the roguelike map.
/// Stored in MapData (ScriptableObject) and instantiated by MapGenerator at run-start.
/// </summary>
[Serializable]
public class MapNodeData
{
    public int nodeIndex;
    public int floor;
    public int column;
    public MapNodeType nodeType;

    /// <summary>Indices of nodes this node connects to on the next floor.</summary>
    public List<int> connectedNodeIndices = new List<int>();

    public bool isVisited;
    public bool isAvailable;

    // ── Encounter payload (only for combat nodes) ───────────────
    public EncounterData encounter;

    // ── Event payload (only for event nodes) ────────────────────
    public EventData eventData;
}
