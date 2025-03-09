using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Handles the spawning and positioning of customer game objects in the world.
/// </summary>
public class CustomerSpawner
{
    private readonly GameObject customerPrefab;
    private readonly float spawnHeightOffset;
    private readonly List<Renderer> tarmacRenderers = new();
    private GameObject currentCustomer;
    private float spawnYPosition;

    /// <summary>
    ///     Initializes a new instance of the CustomerSpawner.
    /// </summary>
    /// <param name="customerPrefab">The prefab to spawn as a customer</param>
    /// <param name="spawnHeightOffset">Height offset from ground where customers should spawn</param>
    public CustomerSpawner(GameObject customerPrefab, float spawnHeightOffset)
    {
        this.customerPrefab = customerPrefab;
        this.spawnHeightOffset = spawnHeightOffset;
        InitializeSpawnArea();
    }

    /// <summary>
    ///     Identifies valid areas for customer spawning based on tagged game objects.
    /// </summary>
    private void InitializeSpawnArea()
    {
        var streets = GameObject.FindGameObjectWithTag("Streets");
        if (streets != null) tarmacRenderers.AddRange(streets.GetComponentsInChildren<Renderer>());

        var ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null)
        {
            var groundRenderer = ground.GetComponent<Renderer>();
            spawnYPosition = groundRenderer.bounds.max.y + spawnHeightOffset;
        }
    }

    /// <summary>
    ///     Spawns a new customer at a random valid position.
    /// </summary>
    public void SpawnCustomer()
    {
        if (tarmacRenderers.Count == 0 || customerPrefab == null) return;

        var spawnPosition = GetSpawnPosition();
        currentCustomer = Object.Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
    }

    /// <summary>
    ///     Destroys the current customer game object.
    /// </summary>
    public void DestroyCurrentCustomer()
    {
        if (currentCustomer != null) Object.Destroy(currentCustomer);
    }

    /// <summary>
    ///     Returns the current customer's position or Vector3.zero if no customer exists.
    /// </summary>
    /// <returns>The position of the current customer</returns>
    public Vector3 GetCurrentCustomerPosition()
    {
        return currentCustomer != null ? currentCustomer.transform.position : Vector3.zero;
    }

    /// <summary>
    ///     Determines a valid spawn position by selecting a random point on a tarmac surface.
    /// </summary>
    /// <returns>A valid spawn position</returns>
    private Vector3 GetSpawnPosition()
    {
        var chosenTarmacRenderer = tarmacRenderers[Random.Range(0, tarmacRenderers.Count)];
        var position = GetRandomPointInBounds(chosenTarmacRenderer.bounds);
        position.y = spawnYPosition;
        return position;
    }

    /// <summary>
    ///     Gets a random point within the specified bounds.
    /// </summary>
    /// <param name="bounds">The bounds to get a random point within</param>
    /// <returns>A random point within the bounds</returns>
    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}