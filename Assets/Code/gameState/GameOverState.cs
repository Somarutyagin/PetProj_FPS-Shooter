using UnityEngine;

public class GameOverState : IGameState
{
    private GameStateManager _gameStateManager;

    public GameOverState(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Enter()
    {
        Time.timeScale = 0f;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //GameStateManager.Instance.ChangeState(new MenuState());
        }
    }
    public void Exit()
    {
    }
}