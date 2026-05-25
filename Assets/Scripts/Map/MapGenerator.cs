using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    private const int MaxConnectionsPerNode = 3;

    public static List<MapNodeData> Generate(MapConfig config, int seed = -1)
    {
        if (seed < 0) seed = Random.Range(0, int.MaxValue);
        Random.InitState(seed);

        int totalFloors = config.floorCount + 1;
        List<MapNodeData> allNodes = new List<MapNodeData>();
        int nodeIndex = 0;

        List<List<MapNodeData>> floors = new List<List<MapNodeData>>();
        for (int f = 0; f < totalFloors; f++)
        {
            List<MapNodeData> floorNodes = new List<MapNodeData>();
            bool isBossFloor = (f == totalFloors - 1);
            int columns = isBossFloor ? 1 : Mathf.Min(config.columnsPerFloor, 7);

            for (int c = 0; c < columns; c++)
            {
                MapNodeData node = new MapNodeData
                {
                    nodeIndex = nodeIndex++,
                    floor = f,
                    column = c,
                    nodeType = PickNodeType(config, f, isBossFloor),
                    isVisited = false,
                    isAvailable = (f == 0),
                };

                if (!isBossFloor)
                {
                    node.encounter = PickEncounter(config, node.nodeType);
                    if (node.nodeType == MapNodeType.Event)
                        node.eventData = PickEvent(config);
                }
                else
                {
                    node.encounter = config.bossEncounter;
                }

                floorNodes.Add(node);
                allNodes.Add(node);
            }

            floors.Add(floorNodes);
        }

        ConnectFloors(floors);

        return allNodes;
    }

    private static MapNodeType PickNodeType(MapConfig config, int floor, bool isBoss)
    {
        if (isBoss) return MapNodeType.Boss;
        if (config.eliteGuaranteedFloors.Contains(floor + 1)) return MapNodeType.Elite;

        float total = config.weightNormal + config.weightElite + config.weightShop
                    + config.weightEvent + config.weightRest;
        float roll = Random.Range(0f, total);

        if (roll < config.weightNormal) return MapNodeType.Normal;
        roll -= config.weightNormal;
        if (roll < config.weightElite) return MapNodeType.Elite;
        roll -= config.weightElite;
        if (roll < config.weightShop) return MapNodeType.Shop;
        roll -= config.weightShop;
        if (roll < config.weightEvent) return MapNodeType.Event;
        return MapNodeType.Rest;
    }

    private static EncounterData PickEncounter(MapConfig config, MapNodeType type)
    {
        List<EncounterData> pool = type == MapNodeType.Elite
            ? config.eliteEncounters
            : config.normalEncounters;

        if (pool == null || pool.Count == 0) return null;
        return pool[Random.Range(0, pool.Count)];
    }

    private static EventData PickEvent(MapConfig config)
    {
        if (config.events == null || config.events.Count == 0) return null;
        return config.events[Random.Range(0, config.events.Count)];
    }

    private static void ConnectFloors(List<List<MapNodeData>> floors)
    {
        for (int f = 0; f < floors.Count - 1; f++)
        {
            List<MapNodeData> current = floors[f];
            List<MapNodeData> next = floors[f + 1];

            bool[] nextConnected = new bool[next.Count];

            foreach (MapNodeData node in current)
            {
                int connections = Random.Range(1, Mathf.Min(MaxConnectionsPerNode, next.Count) + 1);
                HashSet<int> chosen = new HashSet<int>();

                int preferred = Mathf.Clamp(node.column, 0, next.Count - 1);
                chosen.Add(preferred);
                nextConnected[preferred] = true;

                while (chosen.Count < connections)
                {
                    int idx = Random.Range(0, next.Count);
                    chosen.Add(idx);
                    nextConnected[idx] = true;
                }

                foreach (int idx in chosen)
                    node.connectedNodeIndices.Add(next[idx].nodeIndex);
            }

            for (int i = 0; i < next.Count; i++)
            {
                if (!nextConnected[i])
                {
                    MapNodeData parent = current[Random.Range(0, current.Count)];
                    if (!parent.connectedNodeIndices.Contains(next[i].nodeIndex))
                        parent.connectedNodeIndices.Add(next[i].nodeIndex);
                }
            }
        }
    }
}
