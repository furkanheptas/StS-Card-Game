using UnityEngine;
using UnityEngine.InputSystem;

public class CardAimState : CardState
{
    private const int AimSortingOrder = 150;

    private Camera mainCamera;
    private LineRenderer aimLine;

    public CardAimState(Card card, CardStateMachine stateMachine) : base(card, stateMachine)
    {
        mainCamera = Camera.main;
    }

    public override void OnEnter()
    {
        card.AimTarget = null;
        card.SetSortingOrder(AimSortingOrder);
        card.Animator.StopAllAnimations();

        aimLine = card.AimLine;
        if (aimLine != null)
        {
            aimLine.positionCount = 2;
            aimLine.enabled = true;
        }
    }

    public override void Update()
    {
        if (aimLine == null) return;

        Vector3 mouseWorld = GetMouseWorldPosition();
        aimLine.SetPosition(0, card.transform.position);
        aimLine.SetPosition(1, mouseWorld);
    }

    public override void OnExit()
    {
        if (aimLine != null) aimLine.enabled = false;
    }

    public override void OnEndDrag()
    {
        Vector2 mouseWorld = GetMouseWorldPosition();
        card.AimTarget = Physics2D.OverlapPoint(mouseWorld, card.EnemyLayerMask);
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
