using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Renders a single map node on-screen and handles click interaction.
/// </summary>
public class MapNodeView : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private TextMeshPro labelText;

    [Header("Node Type Icons")]
    [SerializeField] private Sprite normalIcon;
    [SerializeField] private Sprite eliteIcon;
    [SerializeField] private Sprite shopIcon;
    [SerializeField] private Sprite eventIcon;
    [SerializeField] private Sprite bossIcon;
    [SerializeField] private Sprite restIcon;

    [Header("State Colors")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color visitedColor = Color.gray;
    [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    private MapNodeData nodeData;
    private Action<MapNodeData> onClickCallback;

    /// <summary>Bind data and click callback to this view.</summary>
    public void Initialize(MapNodeData data, Action<MapNodeData> onClick)
    {
        nodeData = data;
        onClickCallback = onClick;

        SetIcon(data.nodeType);
        if (labelText != null)
            labelText.text = data.nodeType.ToString();

        UpdateVisualState(data);
    }

    /// <summary>Refresh color/opacity based on node availability.</summary>
    public void UpdateVisualState(MapNodeData data)
    {
        nodeData = data;
        Color c = data.isVisited ? visitedColor
                : data.isAvailable ? availableColor
                : lockedColor;

        if (backgroundRenderer != null) backgroundRenderer.color = c;
        if (iconRenderer != null) iconRenderer.color = c;
    }

    private void SetIcon(MapNodeType type)
    {
        if (iconRenderer == null) return;
        iconRenderer.sprite = type switch
        {
            MapNodeType.Normal => normalIcon,
            MapNodeType.Elite  => eliteIcon,
            MapNodeType.Shop   => shopIcon,
            MapNodeType.Event  => eventIcon,
            MapNodeType.Boss   => bossIcon,
            MapNodeType.Rest   => restIcon,
            _                  => null,
        };
    }

    /// <summary>
    /// Called by MapManager when a confirmed click hits this node's collider.
    /// </summary>
    public void OnClicked()
    {
        Debug.Log($"[MapNodeView] Clicked node #{nodeData?.nodeIndex} | isAvailable: {nodeData?.isAvailable} | Type: {nodeData?.nodeType}");

        if (nodeData is { isAvailable: true })
            onClickCallback?.Invoke(nodeData);
        else
            Debug.LogWarning($"[MapNodeView] Node #{nodeData?.nodeIndex} is NOT available, ignoring click.");
    }
}
