using UnityEngine;

public class PlayingState : IGameState
{
    private GameStateManager _gameStateManager;

    public PlayingState(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void Enter()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //_gameStateManager.ChangeState(new PausedState());
        }
    }
    public void Exit()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
}