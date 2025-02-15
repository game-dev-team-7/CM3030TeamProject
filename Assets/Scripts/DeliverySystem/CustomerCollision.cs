using UnityEngine;

public class CustomerCollision : MonoBehaviour
{
    private CustomerManager customerManager;

    private void Start()
    {
        customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager == null) Debug.LogError("CustomerManager not found in the scene!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        customerManager?.CompleteDelivery();
    }
}