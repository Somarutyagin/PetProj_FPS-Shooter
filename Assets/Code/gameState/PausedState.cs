using UnityEngine;

public class PausedState : IGameState
{
    private GameStateManager _gameStateManager;

    public PausedState(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Enter()
    {
        Time.timeScale = 0f;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //_gameStateManager.ChangeState(new PlayingState());
        }
    }
    public void Exit()
    {
    }
}