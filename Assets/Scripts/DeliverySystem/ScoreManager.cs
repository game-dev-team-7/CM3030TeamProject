using UnityEngine;

/// <summary>
///     Manages player score and consecutive delivery streak.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    ///     Gets the current total score.
    /// </summary>
    public int CurrentScore { get; private set; }

    /// <summary>
    ///     Gets the current consecutive delivery streak.
    /// </summary>
    public int CurrentStreak { get; private set; }

    /// <summary>
    ///     Increments the player's score based on current streak.
    ///     The score added is equal to the current streak value,
    ///     creating an incentive for consecutive successful deliveries.
    /// </summary>
    public void IncrementScore()
    {
        CurrentStreak++;
        CurrentScore += CurrentStreak;
    }

    /// <summary>
    ///     Resets the delivery streak to zero when a delivery fails.
    /// </summary>
    public void ResetStreak()
    {
        CurrentStreak = 0;
    }
}