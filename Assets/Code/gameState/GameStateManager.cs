using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private IGameState currentState;

    private void Start()
    {
        ChangeState(new PlayingState(this));
    }
    private void Update()
    {
        currentState?.Update();
    }
    public void ChangeState(IGameState newState)
    {
        currentState?.Exit();

        currentState = newState;
        currentState.Enter();
    }
}