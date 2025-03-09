using UnityEngine;

/// <summary>
///     Handles collision detection between the player and customer for delivery completion.
/// </summary>
public class CustomerCollision : MonoBehaviour
{
    private CustomerManager customerManager;

    /// <summary>
    ///     Initializes references to required components.
    /// </summary>
    private void Start()
    {
        customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager == null) Debug.LogError("CustomerManager not found in the scene!");
    }

    /// <summary>
    ///     Detects when player enters the customer's trigger collider and completes the delivery.
    /// </summary>
    /// <param name="other">The collider that entered this trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        customerManager?.CompleteDelivery();
    }
}