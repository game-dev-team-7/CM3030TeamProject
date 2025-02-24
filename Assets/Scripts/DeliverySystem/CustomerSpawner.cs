using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner
{
    private readonly GameObject customerPrefab;
    private readonly float spawnHeightOffset;
    private GameObject currentCustomer;
    private readonly List<Renderer> tarmacRenderers = new();
    private float spawnYPosition;

    public CustomerSpawner(GameObject customerPrefab, float spawnHeightOffset)
    {
        this.customerPrefab = customerPrefab;
        this.spawnHeightOffset = spawnHeightOffset;
        InitializeSpawnArea();
    }

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

    public void SpawnCustomer()
    {
        if (tarmacRenderers.Count == 0 || customerPrefab == null) return;

        var spawnPosition = GetSpawnPosition();
        currentCustomer = Object.Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
    }

    public void DestroyCurrentCustomer()
    {
        if (currentCustomer != null) Object.Destroy(currentCustomer);
    }

    public Vector3 GetCurrentCustomerPosition()
    {
        return currentCustomer != null ? currentCustomer.transform.position : Vector3.zero;
    }

    private Vector3 GetSpawnPosition()
    {
        var chosenTarmacRenderer = tarmacRenderers[Random.Range(0, tarmacRenderers.Count)];
        var position = GetRandomPointInBounds(chosenTarmacRenderer.bounds);
        position.y = spawnYPosition;
        return position;
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}