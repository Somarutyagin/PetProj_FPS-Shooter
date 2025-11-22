using UnityEngine;

public class MenuState : IGameState
{
    private GameStateManager _gameStateManager;
    
    public MenuState(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Enter()
    {
        Time.timeScale = 0f;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //_gameStateManager.ChangeState(new PlayingState());
        }
    }
    public void Exit()
    {
    }
}