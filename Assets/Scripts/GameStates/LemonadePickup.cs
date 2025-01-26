using UnityEngine;


public class LemonadePickup: MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTemperature playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null)
            {
                playerTemp.ApplyDrinkEffect(DrinkType.Lemonade);
                Destroy(gameObject); // Remove the item from the scene
            }
        }
    }
}