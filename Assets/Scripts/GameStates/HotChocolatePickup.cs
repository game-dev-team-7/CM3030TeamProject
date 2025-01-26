using UnityEngine;

public class HotChocolatePickup: MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTemperature playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null)
            {
                playerTemp.ApplyDrinkEffect(DrinkType.HotChocolate);
                Destroy(gameObject); // Remove the item from the scene
            }
        }
    }
}