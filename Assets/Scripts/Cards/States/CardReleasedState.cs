using UnityEngine;

public class CardReleasedState : CardState
{
    public CardReleasedState(Card card, CardStateMachine stateMachine) : base(card, stateMachine)
    {
    }

    public override void OnEnter()
    {
        bool requiresTarget = card.Data != null && card.Data.targetType == TargetType.SingleEnemy;
        if (requiresTarget && card.AimTarget == null)
        {
            stateMachine.ChangeState(card.IdleState);
            return;
        }

        bool cardPlayed = card.TryPlayCard();
        stateMachine.ChangeState(cardPlayed ? (CardState)card.PlayedState : card.IdleState);
    }
}
