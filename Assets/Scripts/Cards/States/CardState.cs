using UnityEngine;

public abstract class CardState
{
    protected Card card;
    protected CardStateMachine stateMachine;

    public CardState(Card card, CardStateMachine stateMachine)
    {
        this.card = card;
        this.stateMachine = stateMachine;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    public virtual void OnPointerEnter() { }
    public virtual void OnPointerExit() { }
    public virtual void OnPointerDown() { }
    public virtual void OnPointerUp() { }
    public virtual void OnBeginDrag() { }
    public virtual void OnDrag() { }
    public virtual void OnEndDrag() { }
}
