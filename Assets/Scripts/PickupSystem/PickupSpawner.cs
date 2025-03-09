using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     Handles the spawning of pickup items in the game world.
///     Identifies valid spawn locations on tarmac/plaza areas and positions pickups accordingly.
/// </summary>
public class PickupSpawner
{
    /// <summary>
    ///     Height above the ground at which pickups will spawn
    /// </summary>
    private readonly float spawnHeight;

    /// <summary>
    ///     Collection of renderers representing tarmac/plaza areas where pickups can spawn
    /// </summary>
    private readonly List<Renderer> tarmacRenderers = new();

    /// <summary>
    ///     Initializes a new instance of the PickupSpawner class.
    /// </summary>
    /// <param name="spawnHeight">Height above ground for spawned objects</param>
    public PickupSpawner(float spawnHeight)
    {
        this.spawnHeight = spawnHeight;
        InitializeTarmacRenderers();
    }

    /// <summary>
    ///     Finds and caches all renderer components tagged as "Plaza" in the "InnerBlocks" GameObject.
    ///     These renderers define the areas where pickups can spawn.
    /// </summary>
    private void InitializeTarmacRenderers()
    {
        var innerBlocks = GameObject.FindGameObjectWithTag("InnerBlocks");
        if (innerBlocks == null)
        {
            Debug.LogError(
                "[PickupSpawner] InnerBlocks not found. Make sure there's a GameObject tagged as 'InnerBlocks' in the scene.");
            return;
        }

        tarmacRenderers.AddRange(innerBlocks.GetComponentsInChildren<Transform>()
            .Where(t => t.CompareTag("Plaza"))
            .SelectMany(t => t.GetComponentsInChildren<Renderer>()));

        if (tarmacRenderers.Count == 0)
            Debug.LogWarning("[PickupSpawner] No tarmac/plaza renderers found. Pickups will not spawn correctly.");
    }

    /// <summary>
    ///     Spawns a pickup of the specified type at a random valid location.
    /// </summary>
    /// <param name="type">The type of pickup to spawn</param>
    /// <param name="prefab">The prefab to instantiate</param>
    public void SpawnPickup(string type, GameObject prefab)
    {
        var position = GetSpawnPosition();
        Object.Instantiate(prefab, position, Quaternion.identity);
    }

    /// <summary>
    ///     Calculates a random valid spawn position on one of the tarmac/plaza areas.
    /// </summary>
    /// <returns>A Vector3 position to spawn a pickup</returns>
    public Vector3 GetSpawnPosition()
    {
        if (tarmacRenderers.Count == 0)
        {
            Debug.LogWarning("[PickupSpawner] Cannot find spawn position: no tarmac renderers available");
            return Vector3.zero;
        }

        var chosenRenderer = tarmacRenderers[Random.Range(0, tarmacRenderers.Count)];
        var position = GetRandomPointInBounds(chosenRenderer.bounds);
        position.y = spawnHeight;
        return position;
    }

    /// <summary>
    ///     Gets a random point within the specified bounds.
    /// </summary>
    /// <param name="bounds">The bounds to select a point within</param>
    /// <returns>A random Vector3 point within the bounds</returns>
    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}