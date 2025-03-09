using UnityEngine;

/// <summary>
///     Manages the lifespan of pickup items in the game world.
///     After the expiration time elapses, the item can be destroyed and respawned elsewhere.
/// </summary>
public class PickupTimer : MonoBehaviour
{
    /// <summary>
    ///     Time in seconds until the pickup is considered expired
    /// </summary>
    private float timer;

    /// <summary>
    ///     Updates the timer each frame, decreasing it by the time that has passed.
    /// </summary>
    private void Update()
    {
        timer -= Time.deltaTime;
    }

    /// <summary>
    ///     Sets the expiration time for this pickup item.
    /// </summary>
    /// <param name="time">Time in seconds before expiration</param>
    public void SetExpirationTime(float time)
    {
        timer = time;
    }

    /// <summary>
    ///     Checks if the pickup's timer has expired.
    /// </summary>
    /// <returns>True if the timer is less than or equal to zero, false otherwise</returns>
    public bool IsExpired()
    {
        return timer <= 0;
    }
}