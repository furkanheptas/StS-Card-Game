using UnityEngine;

public class CardReleasedState : CardState
{
    private const float PlayThresholdScreenFraction = 0.45f;

    public CardReleasedState(Card card, CardStateMachine stateMachine) : base(card, stateMachine) { }

    public override void OnEnter()
    {
        bool isTargeted = card.Data != null && card.Data.targetType == TargetType.SingleEnemy;

        if (isTargeted)
        {
            if (card.AimTarget == null)
            {
                stateMachine.ChangeState(card.IdleState);
                return;
            }
        }
        else
        {
            Camera cam = card.RootCanvas?.worldCamera ?? Camera.main;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, card.RectTransform.position);
            if (screenPos.y < Screen.height * PlayThresholdScreenFraction)
            {
                stateMachine.ChangeState(card.IdleState);
                return;
            }
        }

        bool played = card.TryPlayCard();
        stateMachine.ChangeState(played ? (CardState)card.PlayedState : card.IdleState);
    }
}
