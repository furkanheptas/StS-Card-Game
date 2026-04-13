using UnityEngine;

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


    public void Update()
    {
        if (Time.timeScale == 0) return; 
        currentState?.Update();
    }

    public void OnPointerEnter()
    {
        if (Time.timeScale == 0) return; 
        currentState?.OnPointerEnter();
    }

    public void OnPointerExit()
    {
        if (Time.timeScale == 0) return;
        currentState?.OnPointerExit();
    }

    public void OnPointerDown()
    {
        if (Time.timeScale == 0) return;
        currentState?.OnPointerDown();
    }

    public void OnPointerUp()
    {
        if (Time.timeScale == 0) return;
        currentState?.OnPointerUp();
    }

    public void OnBeginDrag()
    {
        if (Time.timeScale == 0) return;
        currentState?.OnBeginDrag();
    }

    public void OnDrag()
    {
        if (Time.timeScale == 0) return;
        currentState?.OnDrag();
    }

    public void OnEndDrag()
    {
        if (Time.timeScale == 0) return;
        currentState?.OnEndDrag();
    }
}