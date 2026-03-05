using UnityEngine;
using UnityEngine.InputSystem;

public class CardAimState : CardState
{
    private const int BezierPoints  = 30;
    private const float BaseWidth   = 0.08f;
    private const float MaxDistance = 20f;

    private Camera mainCamera;
    private LineRenderer aimLine;

    public CardAimState(Card card, CardStateMachine stateMachine) : base(card, stateMachine)
    {
        mainCamera = Camera.main;
    }

    public override void OnEnter()
    {
        card.AimTarget = null;
        card.Animator.StopAllAnimations();

        aimLine = card.AimLine;
        if (aimLine != null)
        {
            aimLine.positionCount = BezierPoints;
            aimLine.enabled       = true;
        }
    }

    public override void Update()
    {
        if (aimLine == null) return;

        Vector3 mouseWorld = GetMouseWorldPosition();

        Camera uiCamera = card.RootCanvas?.worldCamera ?? Camera.main;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            card.RectTransform,
            RectTransformUtility.WorldToScreenPoint(uiCamera, card.RectTransform.position),
            uiCamera,
            out Vector3 cardWorld);

        float dist    = Vector3.Distance(cardWorld, mouseWorld);
        Vector3 mid   = (cardWorld + mouseWorld) * 0.5f + Vector3.up * dist * 0.4f;
        float width   = Mathf.Lerp(BaseWidth, BaseWidth * 0.4f, dist / MaxDistance);
        aimLine.startWidth = width;
        aimLine.endWidth   = width * 0.5f;

        for (int i = 0; i < BezierPoints; i++)
        {
            float t = i / (float)(BezierPoints - 1);
            aimLine.SetPosition(i, Bezier(cardWorld, mid, mouseWorld, t));
        }
    }

    public override void OnExit()
    {
        if (aimLine != null) aimLine.enabled = false;
    }

    public override void OnEndDrag()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld  = mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, Mathf.Abs(mainCamera.transform.position.z)));

        card.AimTarget = Physics2D.OverlapPoint(mouseWorld, card.EnemyLayerMask);
        stateMachine.ChangeState(card.ReleasedState);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        return mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, Mathf.Abs(mainCamera.transform.position.z)));
    }

    private static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float u = 1f - t;
        return u * u * a + 2f * u * t * b + t * t * c;
    }
}
