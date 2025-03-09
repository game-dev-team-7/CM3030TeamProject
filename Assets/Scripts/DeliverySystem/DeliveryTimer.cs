using UnityEngine;

/// <summary>
///     Manages the countdown timer for deliveries.
/// </summary>
public class DeliveryTimer
{
    /// <summary>
    ///     Gets the remaining time on the current delivery timer.
    /// </summary>
    public float RemainingTime { get; private set; }

    /// <summary>
    ///     Initializes the timer with the specified duration.
    /// </summary>
    /// <param name="duration">The duration in seconds for the timer</param>
    public void StartTimer(float duration)
    {
        RemainingTime = duration;
    }

    /// <summary>
    ///     Decrements the timer based on elapsed real time.
    ///     Should be called each frame in Update().
    /// </summary>
    public void UpdateTimer()
    {
        if (RemainingTime > 0) RemainingTime -= Time.deltaTime;
    }

    /// <summary>
    ///     Checks if the timer has expired.
    /// </summary>
    /// <returns>True if the timer has reached zero or below, false otherwise</returns>
    public bool IsTimeExpired()
    {
        return RemainingTime <= 0;
    }
}