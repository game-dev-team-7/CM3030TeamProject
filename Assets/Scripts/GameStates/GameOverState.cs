using UnityEngine;

/// <summary>
///     Handles the game state when the player has lost the game.
///     Manages end-game behavior and waits for player interaction with the game over UI.
/// </summary>
public class GameOverState : BaseState
{
    /// <summary>
    ///     Initializes a new instance of the GameOverState.
    /// </summary>
    /// <param name="fsm">Reference to the game's finite state machine</param>
    public GameOverState(GameFSM fsm) : base(fsm)
    {
    }

    /// <summary>
    ///     Called when entering the game over state.
    ///     Performs necessary setup for the game over scenario.
    ///     The actual UI display is handled by InGameMenuManager.
    /// </summary>
    public override void Enter()
    {
        Debug.Log("Entering Game Over State");
        // Optionally stop any ongoing processes or animations here.
    }

    /// <summary>
    ///     Update method for the game over state.
    ///     No active game updates occur during the game over state.
    /// </summary>
    public override void Update()
    {
        // No game updates – waiting for the player's input on the GameOver panel.
    }

    /// <summary>
    ///     Called when exiting the game over state.
    ///     Typically occurs when restarting the game.
    /// </summary>
    public override void Exit()
    {
        Debug.Log("Exiting Game Over State");
    }
}