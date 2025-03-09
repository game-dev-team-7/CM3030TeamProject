using UnityEngine;

/// <summary>
///     Handles the pickup behavior for T-shirt items.
///     T-shirts provide moderate temperature protection suited for warmer weather.
/// </summary>
public class TShirtPickup : MonoBehaviour
{
    /// <summary>
    ///     Item type identifier used by the PlayerPickupSounds system to play the appropriate sound
    /// </summary>
    public string itemType = "TShirt";

    /// <summary>
    ///     Triggered when another collider enters this object's trigger collider.
    ///     If the entering object is the player, applies the T-shirt clothing effect
    ///     and plays the appropriate pickup sound.
    /// </summary>
    /// <param name="other">The collider that entered this object's trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply the T-shirt clothing effect
            var playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null) playerTemp.ApplyClothingResistance(ClothingType.TShirt);

            // Play the pickup sound through the player
            var playerSounds = other.GetComponent<PlayerPickupSounds>();
            if (playerSounds != null) playerSounds.PlayPickupSound(itemType);

            // Destroy the pickup with a slight delay for sound effect
            Destroy(gameObject, 0.1f);
        }
    }
}