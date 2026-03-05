using UnityEngine;

public class CardHoverState : CardState
{
    private const float HoverScale      = 1.12f;
    private const float HoverLiftPixels = 60f;
    private const float HoverDuration   = 0.15f;

    private int originalSiblingIndex;

    public CardHoverState(Card card, CardStateMachine stateMachine) : base(card, stateMachine) { }

    public override void OnEnter()
    {
        originalSiblingIndex = card.RectTransform.GetSiblingIndex();
        card.RectTransform.SetAsLastSibling();

        Vector2 hoverPos = card.HandAnchoredPosition + Vector2.up * HoverLiftPixels;
        card.Animator.MoveTo(hoverPos, HoverDuration);
        card.Animator.RotateTo(Quaternion.identity, HoverDuration);
        card.Animator.ScaleTo(Vector3.one * HoverScale, HoverDuration);
    }

    public override void OnExit()
    {
        card.RectTransform.SetSiblingIndex(originalSiblingIndex);
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
