public class CardStateMachine
{
    private CardState currentState;

    public CardState CurrentState => currentState;

    public void Initialize(CardState startingState)
    {
        currentState = startingState;
        currentState.OnEnter();
    }

    public void ChangeState(CardState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    public void Update() => currentState?.Update();

    public void OnPointerEnter() => currentState?.OnPointerEnter();
    public void OnPointerExit()  => currentState?.OnPointerExit();
    public void OnPointerDown()  => currentState?.OnPointerDown();
    public void OnPointerUp()    => currentState?.OnPointerUp();
    public void OnBeginDrag()    => currentState?.OnBeginDrag();
    public void OnDrag()         => currentState?.OnDrag();
    public void OnEndDrag()      => currentState?.OnEndDrag();
}
