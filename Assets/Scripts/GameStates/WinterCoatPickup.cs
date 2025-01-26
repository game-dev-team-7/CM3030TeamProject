using UnityEngine;


public class WinterCoatPickup: MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTemperature playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null)
            {
                playerTemp.ApplyClothingResistance(ClothingType.TShirt);
                Destroy(gameObject); // Remove the item from the scene
            }
        }
    }
}