using UnityEngine;

public class CardIdleState : CardState
{
    private const float ReturnDuration = 0.25f;

    public CardIdleState(Card card, CardStateMachine stateMachine) : base(card, stateMachine) { }

    public override void OnEnter()
    {
        card.Animator.StopAllAnimations();
        card.Animator.MoveTo(card.HandAnchoredPosition, ReturnDuration);
        card.Animator.RotateTo(card.HandRotation, ReturnDuration);
        card.Animator.ScaleTo(Vector3.one, ReturnDuration);
        card.CanvasGroup.blocksRaycasts = true;
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
