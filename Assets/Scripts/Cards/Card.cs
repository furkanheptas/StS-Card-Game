using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CardAnimator))]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Card Data")]
    [SerializeField] private CardData cardData;

    [Header("Targeting")]
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private LayerMask enemyLayerMask;

    private CardStateMachine stateMachine;
    private SpriteRenderer spriteRenderer;
    private CardAnimator cardAnimator;
    private Vector3 originalPosition;
    private int originalSortingOrder;

    public Collider2D AimTarget { get; set; }

    public CardIdleState IdleState { get; private set; }
    public CardHoverState HoverState { get; private set; }
    public CardDragState DragState { get; private set; }
    public CardAimState AimState { get; private set; }
    public CardReleasedState ReleasedState { get; private set; }
    public CardPlayedState PlayedState { get; private set; }

    public CardData Data => cardData;
    public CardAnimator Animator => cardAnimator;
    public LineRenderer AimLine => aimLine;
    public LayerMask EnemyLayerMask => enemyLayerMask;
    public Vector3 OriginalPosition => originalPosition;
    public Vector3 OriginalScale { get; private set; }
    public int OriginalSortingOrder => originalSortingOrder;
    public int SortingOrder => spriteRenderer.sortingOrder;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cardAnimator = GetComponent<CardAnimator>();
        originalPosition = transform.position;
        OriginalScale = transform.localScale;
        originalSortingOrder = spriteRenderer.sortingOrder;

        stateMachine = new CardStateMachine();
        IdleState = new CardIdleState(this, stateMachine);
        HoverState = new CardHoverState(this, stateMachine);
        DragState = new CardDragState(this, stateMachine);
        AimState = new CardAimState(this, stateMachine);
        ReleasedState = new CardReleasedState(this, stateMachine);
        PlayedState = new CardPlayedState(this, stateMachine);

        if (aimLine != null) aimLine.enabled = false;
    }

    private void Start()
    {
        stateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetSortingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }

    public bool TryPlayCard()
    {
        if (cardData == null) return false;

        ManaSystem mana = ManaSystem.Instance;
        if (mana != null && !mana.CanAfford(cardData.manaCost)) return false;

        DropZone zone = DropZone.FindZoneAtPoint(transform.position);
        if (zone == null) return false;

        mana?.SpendMana(cardData.manaCost);
        zone.AcceptCard(this);
        return true;
    }

    public void OnPointerEnter(PointerEventData eventData) => stateMachine.OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => stateMachine.OnPointerExit();
    public void OnBeginDrag(PointerEventData eventData) => stateMachine.OnBeginDrag();
    public void OnDrag(PointerEventData eventData) => stateMachine.OnDrag();
    public void OnEndDrag(PointerEventData eventData) => stateMachine.OnEndDrag();
}
