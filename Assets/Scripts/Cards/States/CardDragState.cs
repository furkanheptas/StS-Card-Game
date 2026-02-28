using UnityEngine;
using UnityEngine.InputSystem;

public class CardDragState : CardState
{
    private Camera mainCamera;
    private Vector3 offset;

    public CardDragState(Card card, CardStateMachine stateMachine) : base(card, stateMachine)
    {
        mainCamera = Camera.main;
    }

    public override void OnEnter()
    {
        card.SetSortingOrder(200);
        Vector3 mousePosition = GetMouseWorldPosition();
        offset = card.transform.position - mousePosition;
    }

    public override void OnDrag()
    {
        Vector3 targetPosition = GetMouseWorldPosition() + offset;
        card.SetPosition(targetPosition);
    }

    public override void OnEndDrag()
    {
        stateMachine.ChangeState(card.ReleasedState);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mousePosition = new Vector3(mouseScreen.x, mouseScreen.y,
            Mathf.Abs(mainCamera.transform.position.z - card.transform.position.z));
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
