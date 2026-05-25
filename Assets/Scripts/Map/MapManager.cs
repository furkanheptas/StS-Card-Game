using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Scene-level manager for the Map scene.
/// Generates (or restores) the map graph, spawns MapNodeView instances, and handles navigation.
/// </summary>
public class MapManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private MapConfig mapConfig;

    [Header("Prefabs")]
    [SerializeField] private MapNodeView nodeViewPrefab;

    [Header("Layout")]
    [SerializeField] private Transform mapRoot;
    [Tooltip("World-space spacing between floors (rows).")]
    [SerializeField] private float floorSpacing = 1.5f;
    [Tooltip("World-space spacing between columns.")]
    [SerializeField] private float columnSpacing = 1.2f;

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer connectionLinePrefab;

    [Header("Abandon")]
    [SerializeField] private Button abandonButton;

    public UnityEvent<MapNodeData> OnNodeSelected;

    private List<MapNodeData> allNodes;
    private readonly Dictionary<int, MapNodeView> nodeViews = new Dictionary<int, MapNodeView>();

    // Click vs drag discrimination
    private Vector2 mouseDownScreenPos;
    private const float DragThresholdPx = 8f;

    private void Start()
    {
        RunData run = RunManager.Instance?.CurrentRun;

        // Restore persisted map or generate a fresh one
        if (run != null && run.mapNodes != null && run.mapNodes.Count > 0)
            allNodes = run.mapNodes;
        else
            allNodes = MapGenerator.Generate(mapConfig);

        if (run != null)
            run.mapNodes = allNodes;

        SpawnNodeViews();
        DrawConnections();
        RefreshAvailability();
        CenterAndBoundCamera();

        abandonButton?.onClick.AddListener(() =>
        {
            SaveCameraState();
            RunManager.Instance?.AbandonRun();
        });
    }

    private void OnDestroy()
    {
        SaveCameraState();
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            mouseDownScreenPos = Mouse.current.position.ReadValue();

        if (!Mouse.current.leftButton.wasReleasedThisFrame) return;

        // Ignore if mouse moved too far — user was panning, not clicking
        Vector2 mouseUpScreenPos = Mouse.current.position.ReadValue();
        if (Vector2.Distance(mouseDownScreenPos, mouseUpScreenPos) > DragThresholdPx) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mouseUpScreenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit == null) return;

        MapNodeView view = hit.GetComponent<MapNodeView>();
        view?.OnClicked();
    }

    // ── Node Spawning ──────────────────────────────────────────

    private void SpawnNodeViews()
    {
        foreach (MapNodeData node in allNodes)
        {
            float x = (node.column - (mapConfig.columnsPerFloor - 1) / 2f) * columnSpacing;
            float y = node.floor * floorSpacing;
            Vector3 pos = mapRoot.position + new Vector3(x, y, 0f);

            MapNodeView view = Instantiate(nodeViewPrefab, pos, Quaternion.identity, mapRoot);
            view.Initialize(node, OnNodeClicked);
            nodeViews[node.nodeIndex] = view;
        }
    }

    private void DrawConnections()
    {
        if (connectionLinePrefab == null) return;

        foreach (MapNodeData node in allNodes)
        {
            foreach (int childIndex in node.connectedNodeIndices)
            {
                if (!nodeViews.TryGetValue(childIndex, out MapNodeView childView)) continue;
                if (!nodeViews.TryGetValue(node.nodeIndex, out MapNodeView parentView)) continue;

                LineRenderer lr = Instantiate(connectionLinePrefab, mapRoot);
                lr.positionCount = 2;
                lr.SetPosition(0, parentView.transform.position);
                lr.SetPosition(1, childView.transform.position);
            }
        }
    }

    // ── Interaction ────────────────────────────────────────────

    private void OnNodeClicked(MapNodeData node)
    {
        Debug.Log($"Node clicked: Index={node.nodeIndex}, Type={node.nodeType}, Floor={node.floor}, Column={node.column}");
        if (!node.isAvailable) return;

        LockFloor(node.floor);
        node.isVisited = true;
        UnlockChildren(node);
        RefreshAvailability();

        OnNodeSelected?.Invoke(node);
        SaveCameraState();
        RunManager.Instance?.EnterNode(node);
    }

    /// <summary>Lock all nodes on the given floor so only one path can be taken.</summary>
    private void LockFloor(int floor)
    {
        foreach (MapNodeData n in allNodes)
        {
            if (n.floor == floor && !n.isVisited)
                n.isAvailable = false;
        }
    }

    private void UnlockChildren(MapNodeData parent)
    {
        foreach (int childIndex in parent.connectedNodeIndices)
        {
            MapNodeData child = allNodes.Find(n => n.nodeIndex == childIndex);
            if (child != null) child.isAvailable = true;
        }
    }

    private void RefreshAvailability()
    {
        foreach (var kvp in nodeViews)
        {
            MapNodeData node = allNodes.Find(n => n.nodeIndex == kvp.Key);
            if (node != null) kvp.Value.UpdateVisualState(node);
        }
    }

    // ── Camera ─────────────────────────────────────────────────

    private void SaveCameraState()
    {
        Camera cam = Camera.main;
        if (cam == null || RunManager.Instance == null) return;
        RunManager.Instance.SavedMapCameraPosition = cam.transform.position;
        RunManager.Instance.SavedMapCameraSize     = cam.orthographicSize;
    }

    /// <summary>
    /// Centers the camera on the bottom floors (available nodes) and sets pan/zoom bounds.
    /// Restores the last saved position if returning from a node.
    /// </summary>
    private void CenterAndBoundCamera()
    {
        if (nodeViews.Count == 0) return;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (MapNodeView view in nodeViews.Values)
        {
            Vector3 p = view.transform.position;
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        Camera cam = Camera.main;
        if (cam == null) return;

        MapCameraController controller = cam.GetComponent<MapCameraController>();

        // Restore saved camera state if we're returning from a battle/event
        if (RunManager.Instance?.SavedMapCameraPosition.HasValue == true)
        {
            cam.transform.position    = RunManager.Instance.SavedMapCameraPosition.Value;
            cam.orthographicSize      = RunManager.Instance.SavedMapCameraSize ?? cam.orthographicSize;
        }
        else
        {
            // Fresh run: start at bottom center
            float centerX = (minX + maxX) / 2f;
            cam.transform.position = new Vector3(centerX, minY, cam.transform.position.z);
            cam.orthographicSize   = Mathf.Clamp(floorSpacing * 3f, 4f, 12f);
        }

        controller?.SetBounds(minX, maxX, minY, maxY);
    }

    // ── HUD ────────────────────────────────────────────────────
    // HP/Gold/Floor bilgisi artık RunHUD (DontDestroyOnLoad) üzerinden gösteriliyor.
}
