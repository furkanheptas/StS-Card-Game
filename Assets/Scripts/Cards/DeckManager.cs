using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    public event Action OnHandChanged;
    public event Action OnTurnEnded;

    [Header("Deck Configuration")]
    [SerializeField] private DeckData deckData;
    [SerializeField] private int handSize = 5;

    [Header("Card Spawning")]
    [SerializeField] private Card cardPrefab;
    [SerializeField] private RectTransform handParent;
    [Tooltip("LineRenderer used by all cards during the aim state.")]
    [SerializeField] private LineRenderer sharedAimLine;

    [Header("Pile Views")]
    [SerializeField] private CardPileView drawPileView;
    [SerializeField] private CardPileView discardPileView;

    [Header("Pile Positions (for animation)")]
    [SerializeField] private RectTransform drawPileRect;
    [SerializeField] private RectTransform discardPileRect;

    private readonly List<CardData> drawPile    = new List<CardData>();
    private readonly List<CardData> discardPile = new List<CardData>();
    private readonly List<Card>     hand        = new List<Card>();

    public int DrawPileCount    => drawPile.Count;
    public int DiscardPileCount => discardPile.Count;
    public int HandCount        => hand.Count;
    public IReadOnlyList<Card> Hand => hand;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeDeck();
        StartCoroutine(DelayedInitialDraw());
    }

    private System.Collections.IEnumerator DelayedInitialDraw()
    {
        yield return null;
        ManaSystem.Instance?.StartTurn();
        int extraDraw = RunManager.Instance?.CurrentRun?.GetBonusCardDraw() ?? 0;
        DrawCards(handSize + extraDraw);
    }

    public void InitializeDeck()
    {
        drawPile.Clear();
        discardPile.Clear();

        RunData run = RunManager.Instance?.CurrentRun;
        if (run != null && run.deck.Count > 0)
            drawPile.AddRange(run.deck);
        else if (deckData != null)
            drawPile.AddRange(deckData.cards);

        ShuffleDrawPile();
        RefreshPileViews();
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (drawPile.Count == 0)
            {
                if (discardPile.Count == 0) break;
                RecycleDiscardIntoDrawPile();
            }

            CardData data = drawPile[0];
            drawPile.RemoveAt(0);

            Card card = Instantiate(cardPrefab, handParent);

            if (drawPileRect != null)
                card.RectTransform.anchoredPosition = GetPileLocalPosition(drawPileRect);

            card.SetAimLine(sharedAimLine);
            card.Initialize(data);
            hand.Add(card);
        }

        RefreshPileViews();
        OnHandChanged?.Invoke();
    }

    public void OnCardPlayed(Card card)
    {
        if (!hand.Contains(card)) return;
        hand.Remove(card);
        discardPile.Add(card.Data);
        RefreshPileViews();
        OnHandChanged?.Invoke();
    }

    public void DiscardCard(Card card)
    {
        if (!hand.Contains(card)) return;
        hand.Remove(card);
        discardPile.Add(card.Data);
        Destroy(card.gameObject);
        RefreshPileViews();
        OnHandChanged?.Invoke();
    }

    public void SetHandSize(int size)
    {
        handSize = Mathf.Max(0, size);
    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnRoutine());
    }

    private IEnumerator EndTurnRoutine()
    {
        const float discardDuration = 0.22f;
        const float stagger = 0.06f;

        Vector2 discardPos = GetDiscardPileLocalPosition();
        List<Card> snapshot = new List<Card>(hand);

        for (int i = 0; i < snapshot.Count; i++)
        {
            Card card = snapshot[i];
            card.CanvasGroup.blocksRaycasts = false;

            int index = i;
            float delay = index * stagger;

            card.Animator.MoveTo(discardPos, discardDuration);
            card.Animator.ScaleTo(Vector3.one * 0.15f, discardDuration);
        }

        yield return new WaitForSeconds(discardDuration + snapshot.Count * stagger);

        foreach (Card card in snapshot)
        {
            discardPile.Add(card.Data);
            Destroy(card.gameObject);
        }
        hand.Clear();

        ManaSystem.Instance?.StartTurn();
        DrawCards(handSize);
        OnTurnEnded?.Invoke();
    }

    private void ShuffleDrawPile()
    {
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (drawPile[i], drawPile[j]) = (drawPile[j], drawPile[i]);
        }
    }

    private void RecycleDiscardIntoDrawPile()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDrawPile();
        Debug.Log("[DeckManager] Discard pile shuffled back into draw pile.");
    }

    private void RefreshPileViews()
    {
        drawPileView?.SetCount(drawPile.Count);
        discardPileView?.SetCount(discardPile.Count);
    }

    public Vector2 GetDiscardPileLocalPosition() => GetPileLocalPosition(discardPileRect);

    public Vector2 GetDrawPileLocalPosition() => GetPileLocalPosition(drawPileRect);

    private Vector2 GetPileLocalPosition(RectTransform pileRect)
    {
        if (pileRect == null) return Vector2.zero;

        Canvas canvas = handParent.GetComponentInParent<Canvas>();
        Camera cam = canvas != null ? canvas.worldCamera : Camera.main;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, pileRect.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            handParent, screenPoint, cam, out Vector2 localPoint);

        return localPoint;
    }
}
