using UnityEngine;
using UnityEngine.InputSystem;

public class CardDragState : CardState
{
    private const float DragScale = 1.05f;

    private Vector2 previousMousePosition;

    public CardDragState(Card card, CardStateMachine stateMachine) : base(card, stateMachine) { }

    public override void OnEnter()
    {
        card.RectTransform.SetAsLastSibling();
        card.Animator.StopAllAnimations();
        card.Animator.ScaleTo(Vector3.one * DragScale, 0.1f);
        card.Animator.RotateTo(Quaternion.identity, 0.1f);
        card.CanvasGroup.blocksRaycasts = false;
        previousMousePosition = Mouse.current.position.ReadValue();
    }

    public override void OnExit()
    {
        card.CanvasGroup.blocksRaycasts = true;
    }

    public override void OnDrag()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 delta = (mousePosition - previousMousePosition) / card.RootCanvas.scaleFactor;
        previousMousePosition = mousePosition;
        card.RectTransform.anchoredPosition += delta;
    }

    public override void OnEndDrag()
    {
        stateMachine.ChangeState(card.ReleasedState);
    }
}
