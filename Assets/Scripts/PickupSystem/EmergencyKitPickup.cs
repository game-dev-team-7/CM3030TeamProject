using UnityEngine;

/// <summary>
///     Handles the pickup behavior for emergency kits.
///     Emergency kits immediately reset the player's temperature to the normal level
///     when their temperature becomes dangerously high or low.
/// </summary>
public class EmergencyKitPickup : MonoBehaviour
{
    /// <summary>
    ///     Triggered when another collider enters this object's trigger collider.
    ///     If the entering object is the player, resets their temperature to normal
    ///     and destroys this emergency kit.
    /// </summary>
    /// <param name="other">The collider that entered this object's trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null)
            {
                // Reset the player's temperature to the normal/safe value
                playerTemp.ResetTemperature();

                // Remove the item from the scene after it has been used
                Destroy(gameObject);
            }
        }
    }
}