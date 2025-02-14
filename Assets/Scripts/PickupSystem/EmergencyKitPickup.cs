using UnityEngine;


public class EmergencyKitPickup: MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTemperature playerTemp = other.GetComponent<PlayerTemperature>();
            if (playerTemp != null)
            {
                playerTemp.ResetTemperature();
                Destroy(gameObject); // Remove the item from the scene
            }
        }
    }
}