using System;
using System.Collections.Generic;
using UnityEngine;

public enum MapNodeType { Normal, Elite, Shop, Event, Boss, Rest }

[Serializable]
public class MapNodeData
{
    public int nodeIndex;
    public int floor;
    public int column;
    public MapNodeType nodeType;

    public List<int> connectedNodeIndices = new List<int>();

    public bool isVisited;
    public bool isAvailable;

    public EncounterData encounter;

    public EventData eventData;
}
