using UnityEngine;

public class GameOverState : BaseState
{
    public GameOverState(GameFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entering Game Over State");
        // Optionally stop any ongoing processes or animations here.
    }

    public override void Update()
    {
        // No game updates – waiting for the player's input on the GameOver panel.
    }

    public override void Exit()
    {
        Debug.Log("Exiting Game Over State");
    }
}