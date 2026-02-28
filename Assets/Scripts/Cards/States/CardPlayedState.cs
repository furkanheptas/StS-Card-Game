using UnityEngine;

public class CardPlayedState : CardState
{
    private const float PlayAnimationDuration = 0.25f;

    public CardPlayedState(Card card, CardStateMachine stateMachine) : base(card, stateMachine)
    {
    }

    public override void OnEnter()
    {
        card.SetSortingOrder(300);
        card.Animator.MoveTo(Vector3.zero, PlayAnimationDuration, OnPlayAnimationComplete);
        card.Animator.ScaleTo(Vector3.zero, PlayAnimationDuration);
    }

    private void OnPlayAnimationComplete()
    {
        card.gameObject.SetActive(false);
    }
}
