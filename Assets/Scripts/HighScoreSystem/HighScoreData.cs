using System;

/// <summary>
///     Serializable data structure to store high score information.
///     Used for saving and loading high score data via JSON serialization.
/// </summary>
[Serializable]
public class HighScoreData
{
    /// <summary>
    ///     The player's highest achieved score in the game.
    /// </summary>
    public int highScore;
}