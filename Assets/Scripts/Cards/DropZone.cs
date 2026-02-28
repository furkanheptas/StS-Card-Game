using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour
{
    public event Action<Card> OnCardDropped;

    private Collider2D zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
        zoneCollider.isTrigger = true;
    }

    public bool ContainsPoint(Vector2 worldPoint) => zoneCollider.OverlapPoint(worldPoint);

    public void AcceptCard(Card card)
    {
        OnCardDropped?.Invoke(card);
    }

    public static DropZone FindZoneAtPoint(Vector2 worldPoint)
    {
        DropZone[] allZones = FindObjectsByType<DropZone>(FindObjectsSortMode.None);
        foreach (DropZone zone in allZones)
        {
            if (zone.ContainsPoint(worldPoint))
                return zone;
        }
        return null;
    }
}
