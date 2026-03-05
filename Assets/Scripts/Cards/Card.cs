using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(CardAnimator))]
public class Card : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Visual References")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI manaCostText;

    [Header("Targeting")]
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private LayerMask enemyLayerMask;

    private CardStateMachine stateMachine;
    private CardData cardData;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 handAnchoredPosition;
    private Quaternion handRotation = Quaternion.identity;

    public Collider2D AimTarget { get; set; }

    public CardIdleState     IdleState     { get; private set; }
    public CardHoverState    HoverState    { get; private set; }
    public CardDragState     DragState     { get; private set; }
    public CardAimState      AimState      { get; private set; }
    public CardReleasedState ReleasedState { get; private set; }
    public CardPlayedState   PlayedState   { get; private set; }

    public CardData         Data                 => cardData;
    public CardAnimator     Animator             { get; private set; }
    public RectTransform    RectTransform        => rectTransform;
    public CanvasGroup      CanvasGroup          => canvasGroup;
    public Canvas           RootCanvas           { get; private set; }
    public LineRenderer     AimLine              => aimLine;
    public LayerMask        EnemyLayerMask       => enemyLayerMask;
    public Vector2          HandAnchoredPosition => handAnchoredPosition;
    public Quaternion       HandRotation         => handRotation;
    public CardStateMachine StateMachine         => stateMachine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup   = GetComponent<CanvasGroup>();
        Animator      = GetComponent<CardAnimator>();
        RootCanvas    = GetComponentInParent<Canvas>();

        stateMachine  = new CardStateMachine();
        IdleState     = new CardIdleState(this, stateMachine);
        HoverState    = new CardHoverState(this, stateMachine);
        DragState     = new CardDragState(this, stateMachine);
        AimState      = new CardAimState(this, stateMachine);
        ReleasedState = new CardReleasedState(this, stateMachine);
        PlayedState   = new CardPlayedState(this, stateMachine);

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

    public void SetAimLine(LineRenderer line) => aimLine = line;

    public void Initialize(CardData data)
    {
        cardData = data;
        if (cardNameText    != null) cardNameText.text    = data.cardName;
        if (descriptionText != null) descriptionText.text = data.description;
        if (manaCostText    != null) manaCostText.text    = data.manaCost.ToString();
        if (artworkImage    != null && data.artwork != null) artworkImage.sprite = data.artwork;
    }

    public void SetHandTransform(Vector2 anchoredPosition, Quaternion rotation)
    {
        handAnchoredPosition = anchoredPosition;
        handRotation         = rotation;

        if (stateMachine.CurrentState is CardIdleState)
        {
            Animator.MoveTo(anchoredPosition, 0.2f);
            Animator.RotateTo(rotation, 0.2f);
        }
    }

    public bool TryPlayCard()
    {
        if (cardData == null) return false;

        ManaSystem mana = ManaSystem.Instance;
        if (mana != null && !mana.CanAfford(cardData.manaCost)) return false;

        mana?.SpendMana(cardData.manaCost);
        CardEffectResolver.Resolve(cardData, AimTarget);
        DeckManager.Instance?.OnCardPlayed(this);
        return true;
    }

    public void OnPointerEnter(PointerEventData eventData) => stateMachine.OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData)  => stateMachine.OnPointerExit();
    public void OnPointerDown(PointerEventData eventData)  => stateMachine.OnPointerDown();
    public void OnPointerUp(PointerEventData eventData)    => stateMachine.OnPointerUp();
    public void OnBeginDrag(PointerEventData eventData)    => stateMachine.OnBeginDrag();
    public void OnDrag(PointerEventData eventData)         => stateMachine.OnDrag();
    public void OnEndDrag(PointerEventData eventData)      => stateMachine.OnEndDrag();
}


