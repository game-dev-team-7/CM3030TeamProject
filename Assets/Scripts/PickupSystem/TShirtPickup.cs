using UnityEngine;


public class TShirtPickup: MonoBehaviour
{
    public string itemType = "TShirt";//Item type used in PickupSoundsManager

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTemperature playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null)
            {
                playerTemp.ApplyClothingResistance(ClothingType.TShirt);
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