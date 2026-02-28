using UnityEngine;

public class CardHoverState : CardState
{
    private const float HoverScale = 1.1f;
    private const float HoverLiftAmount = 0.5f;
    private const float HoverDuration = 0.15f;

    public CardHoverState(Card card, CardStateMachine stateMachine) : base(card, stateMachine)
    {
    }

    public override void OnEnter()
    {
        card.SetSortingOrder(100);
        Vector3 hoverPosition = card.OriginalPosition + Vector3.up * HoverLiftAmount;
        card.Animator.MoveTo(hoverPosition, HoverDuration);
        card.Animator.ScaleTo(card.OriginalScale * HoverScale, HoverDuration);
    }

    public override void OnPointerExit()
    {
        stateMachine.ChangeState(card.IdleState);
    }

    public override void OnBeginDrag()
    {
        bool isTargeted = card.Data != null && card.Data.targetType == TargetType.SingleEnemy;
        stateMachine.ChangeState(isTargeted ? card.AimState : (CardState)card.DragState);
    }
}
