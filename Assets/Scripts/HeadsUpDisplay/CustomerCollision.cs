using UnityEngine;

public class CustomerCollision : MonoBehaviour
{
    private CustomerManager customerManager;

    private void Start()
    {
        customerManager = FindObjectOfType<CustomerManager>(); // Finds the CustomerManager in the scene

        if (customerManager == null)
        {
            Debug.LogError("CustomerManager not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached the Customer!");

            if (customerManager != null)
            {
                customerManager.HandlePlayerInteraction(); // Call method in CustomerManager
            }
        }
    }
}
