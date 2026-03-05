using UnityEngine;

public class CardPlayedState : CardState
{
    private const float AnimDuration = 0.3f;

    public CardPlayedState(Card card, CardStateMachine stateMachine) : base(card, stateMachine) { }

    public override void OnEnter()
    {
        card.CanvasGroup.blocksRaycasts = false;
        card.Animator.StopAllAnimations();

        Vector2 discardLocalPos = DeckManager.Instance != null
            ? DeckManager.Instance.GetDiscardPileLocalPosition()
            : Vector2.zero;

        card.Animator.MoveTo(discardLocalPos, AnimDuration);
        card.Animator.ScaleTo(Vector3.one * 0.2f, AnimDuration, OnAnimationComplete);
    }

    private void OnAnimationComplete()
    {
        Object.Destroy(card.gameObject);
    }
}
