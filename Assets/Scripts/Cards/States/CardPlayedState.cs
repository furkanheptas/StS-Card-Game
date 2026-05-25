using UnityEngine;
using System.Collections;

public class CardPlayedState : CardState
{
    private const float CenterDuration = 0.4f; 
    private const float AnimDuration = 0.3f; 

    public CardPlayedState(Card card, CardStateMachine stateMachine) : base(card, stateMachine) { }

    public override void OnEnter()
    {
        card.CanvasGroup.blocksRaycasts = false;
        card.Animator.StopAllAnimations();

        card.RectTransform.SetAsLastSibling(); 
        card.Animator.MoveTo(Vector2.up * 100f, 0.2f);
        card.Animator.ScaleTo(Vector3.one * 1.2f, 0.2f);
        card.Animator.RotateTo(Quaternion.identity, 0.2f);

        card.StartCoroutine(WaitAndDiscard());
    }

    private System.Collections.IEnumerator WaitAndDiscard()
    {
        yield return new WaitForSeconds(CenterDuration);

        Vector2 discardLocalPos = DeckManager.Instance != null
            ? DeckManager.Instance.GetDiscardPileLocalPosition()
            : Vector2.zero;

        card.Animator.MoveTo(discardLocalPos, AnimDuration);
        card.Animator.ScaleTo(Vector3.one * 0.1f, AnimDuration, OnAnimationComplete);
    }

    private void OnAnimationComplete()
    {
        Object.Destroy(card.gameObject);
    }
}