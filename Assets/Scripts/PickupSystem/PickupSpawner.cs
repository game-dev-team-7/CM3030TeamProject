using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupSpawner
{
    private readonly List<Renderer> tarmacRenderers = new();
    private readonly float spawnHeight;

    public PickupSpawner(float spawnHeight)
    {
        this.spawnHeight = spawnHeight;
        InitializeTarmacRenderers();
    }

    private void InitializeTarmacRenderers()
    {
        var innerBlocks = GameObject.FindGameObjectWithTag("InnerBlocks");
        if (innerBlocks == null)
        {
            Debug.LogError("InnerBlocks not found");
            return;
        }

        tarmacRenderers.AddRange(innerBlocks.GetComponentsInChildren<Transform>()
            .Where(t => t.CompareTag("Plaza"))
            .SelectMany(t => t.GetComponentsInChildren<Renderer>()));
    }

    public void SpawnPickup(string type, GameObject prefab)
    {
        var position = GetSpawnPosition();
        Object.Instantiate(prefab, position, Quaternion.identity);
    }

    public Vector3 GetSpawnPosition()
    {
        if (tarmacRenderers.Count == 0) return Vector3.zero;

        var chosenRenderer = tarmacRenderers[Random.Range(0, tarmacRenderers.Count)];
        var position = GetRandomPointInBounds(chosenRenderer.bounds);
        position.y = spawnHeight;
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