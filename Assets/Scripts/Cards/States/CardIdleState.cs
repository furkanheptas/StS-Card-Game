using UnityEngine;

public class CardIdleState : CardState
{
    private const float ReturnDuration = 0.2f;

    private readonly Vector3 originalScale;
    private readonly int originalSortingOrder;

    public CardIdleState(Card card, CardStateMachine stateMachine) : base(card, stateMachine)
    {
        originalScale = card.transform.localScale;
        originalSortingOrder = card.SortingOrder;
    }

    public override void OnEnter()
    {
        card.SetSortingOrder(originalSortingOrder);
        card.Animator.MoveTo(card.OriginalPosition, ReturnDuration);
        card.Animator.ScaleTo(originalScale, ReturnDuration);
    }

    public override void OnPointerEnter()
    {
        stateMachine.ChangeState(card.HoverState);
    }

    public override void OnBeginDrag()
    {
        bool isTargeted = card.Data != null && card.Data.targetType == TargetType.SingleEnemy;
        stateMachine.ChangeState(isTargeted ? card.AimState : (CardState)card.DragState);
    }
}
