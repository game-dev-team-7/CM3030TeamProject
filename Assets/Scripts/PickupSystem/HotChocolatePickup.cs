using UnityEngine;

public class HotChocolatePickup: MonoBehaviour
{
    public string itemType = "HotChocolate";//Item type used in PickupSoundsManager
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTemperature playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null)
            {
                playerTemp.ApplyDrinkEffect(DrinkType.HotChocolate);
            }

            // Play the pickup sound through the player
            PlayerPickupSounds playerSounds = other.GetComponent<PlayerPickupSounds>();
            if (playerSounds != null)
            {
                playerSounds.PlayPickupSound(itemType);
            }

            Destroy(gameObject, 0.1f);
        }
    }
}