using UnityEngine;

/// <summary>
///     Handles the pickup behavior for lemonade items.
///     Lemonade helps cool down the player in hot weather conditions.
/// </summary>
public class LemonadePickup : MonoBehaviour
{
    /// <summary>
    ///     Item type identifier used by the PlayerPickupSounds system to play the appropriate sound
    /// </summary>
    public string itemType = "Lemonade";

    /// <summary>
    ///     Triggered when another collider enters this object's trigger collider.
    ///     If the entering object is the player, applies the lemonade effect
    ///     and plays the appropriate pickup sound.
    /// </summary>
    /// <param name="other">The collider that entered this object's trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply the lemonade temperature effect
            var playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null) playerTemp.ApplyDrinkEffect(DrinkType.Lemonade);

            // Play the pickup sound through the player
            var playerSounds = other.GetComponent<PlayerPickupSounds>();
            if (playerSounds != null) playerSounds.PlayPickupSound(itemType);

            // Destroy the pickup with a slight delay for sound effect
            Destroy(gameObject, 0.1f);
        }
    }
}