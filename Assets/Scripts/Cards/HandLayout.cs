using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HandLayout : MonoBehaviour
{
    [Header("Arc Settings")]
    [SerializeField] private float cardSpacing = 160f;
    [SerializeField] private float maxArcAngle = 24f;
    [SerializeField] private float maxSpreadWidth = 720f;
    [SerializeField] private float arcDepth = 40f;

    private DeckManager deckManager;

    private void Start()
    {
        deckManager = DeckManager.Instance;
        if (deckManager != null)
            deckManager.OnHandChanged += ArrangeCards;

        ArrangeCards();
    }

    private void OnDestroy()
    {
        if (deckManager != null)
            deckManager.OnHandChanged -= ArrangeCards;
    }

    public void ArrangeCards()
    {
        if (deckManager == null) return;

        IReadOnlyList<Card> cards = deckManager.Hand;
        int count = cards.Count;
        if (count == 0) return;

        float totalWidth = Mathf.Min((count - 1) * cardSpacing, maxSpreadWidth);
        float angleRange = Mathf.Min(maxArcAngle, maxArcAngle);

        for (int i = 0; i < count; i++)
        {
            if (cards[i] == null) continue;

            float t = count > 1 ? (float)i / (count - 1) - 0.5f : 0f;

            float xOffset = t * totalWidth;
            float yOffset = -Mathf.Abs(t) * arcDepth * 2f;

            float angle = -t * angleRange;

            Vector2 targetPos = new Vector2(xOffset, yOffset);
            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);

            cards[i].SetHandTransform(targetPos, targetRot);
        }
    }
}
