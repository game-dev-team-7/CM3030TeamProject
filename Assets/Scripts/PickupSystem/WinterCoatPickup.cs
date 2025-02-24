using UnityEngine;


public class CoatPickup : MonoBehaviour
{
    public string itemType = "WinterCoat";//Item type used for pickup sound

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTemperature playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null)
            {
                playerTemp.ApplyClothingResistance(ClothingType.WinterCoat);
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