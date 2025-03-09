using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     Manages the lifecycle of all pickup items in the game world.
///     Handles spawning, respawning, and monitoring pickup items.
///     Also spawns emergency kits when the player's temperature reaches dangerous levels.
/// </summary>
public class PickupManager : MonoBehaviour
{
    [Header("Pickup Prefabs")] [SerializeField]
    private GameObject tShirtPrefab;

    [SerializeField] private GameObject coatPrefab;
    [SerializeField] private GameObject hotChocolatePrefab;
    [SerializeField] private GameObject lemonadePrefab;
    [SerializeField] private GameObject emergencyKitPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Time in seconds before a pickup disappears and respawns elsewhere")]
    [SerializeField]
    private float expirationTime = 30f;

    [Tooltip("Distance in front of player to spawn emergency kit")] [SerializeField]
    private float emergencyKitDistance = 60f;

    [Tooltip("Height above ground to spawn pickups")] [SerializeField]
    private float spawnHeight = 2f;

    /// <summary>Dictionary tracking all active pickups by type</summary>
    private readonly Dictionary<string, GameObject> activePickups = new();

    /// <summary>Flag to track if an emergency kit has already been spawned</summary>
    private bool emergencyKitSpawned;

    /// <summary>Reference to the player GameObject</summary>
    private GameObject player;

    /// <summary>Reference to the player's temperature component</summary>
    private PlayerTemperature playerTemperature;

    /// <summary>Cached positions of all road tiles for emergency kit spawning</summary>
    private Vector3[] roadTilePositions;

    /// <summary>Pickup spawner utility</summary>
    private PickupSpawner spawner;

    /// <summary>
    ///     Initializes components and spawns initial pickups.
    /// </summary>
    private void Start()
    {
        InitializeComponents();
        spawner = new PickupSpawner(spawnHeight);
        SpawnInitialPickups();

        // Cache all RoadTile positions at the start for better performance
        roadTilePositions = GameObject.FindGameObjectsWithTag("RoadTile")
            .Select(tile => tile.transform.position)
            .ToArray();
    }

    /// <summary>
    ///     Updates pickup states each frame.
    /// </summary>
    private void Update()
    {
        HandleExpiredPickups();
        CheckEmergencyKitSpawn();
    }

    /// <summary>
    ///     Initializes references to required components.
    /// </summary>
    private void InitializeComponents()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[PickupManager] Player not found. Make sure the player has the 'Player' tag.");
            enabled = false;
            return;
        }

        playerTemperature = player.GetComponent<PlayerTemperature>();
        if (playerTemperature == null)
        {
            Debug.LogError("[PickupManager] PlayerTemperature component not found on player.");
            enabled = false;
        }
    }

    /// <summary>
    ///     Spawns the initial set of pickups in the game world.
    /// </summary>
    private void SpawnInitialPickups()
    {
        SpawnPickup("TShirt", tShirtPrefab);
        SpawnPickup("WinterCoat", coatPrefab);
        SpawnPickup("HotChocolate", hotChocolatePrefab);
        SpawnPickup("Lemonade", lemonadePrefab);
    }

    /// <summary>
    ///     Checks for and respawns any pickups that have expired.
    /// </summary>
    private void HandleExpiredPickups()
    {
        var expiredPickups = new Dictionary<string, GameObject>(activePickups);
        foreach (var pickup in expiredPickups)
            if (IsPickupExpired(pickup.Value))
                RemoveAndRespawnPickup(pickup.Key);
    }

    /// <summary>
    ///     Checks if a pickup has expired or been destroyed.
    /// </summary>
    /// <param name="pickup">The pickup GameObject to check</param>
    /// <returns>True if the pickup has expired or been destroyed, false otherwise</returns>
    private bool IsPickupExpired(GameObject pickup)
    {
        if (pickup == null) return true;
        var timer = pickup.GetComponent<PickupTimer>();
        return timer != null && timer.IsExpired();
    }

    /// <summary>
    ///     Removes an expired pickup and spawns a new one of the same type.
    /// </summary>
    /// <param name="type">The type of pickup to respawn</param>
    private void RemoveAndRespawnPickup(string type)
    {
        if (activePickups.TryGetValue(type, out var pickup))
        {
            if (pickup != null) Destroy(pickup);
            activePickups.Remove(type);
            SpawnPickup(type, GetPrefabForType(type));
        }
    }

    /// <summary>
    ///     Checks if an emergency kit needs to be spawned based on player's temperature.
    /// </summary>
    private void CheckEmergencyKitSpawn()
    {
        if (emergencyKitSpawned) return;

        var temp = playerTemperature.bodyTemperature;
        // Spawn emergency kit if temperature is at 85% of either extreme
        if (temp < playerTemperature.minTemperature * 0.85f ||
            temp > playerTemperature.maxTemperature * 0.85f)
        {
            SpawnEmergencyKit();
            emergencyKitSpawned = true;
        }
    }

    /// <summary>
    ///     Spawns an emergency kit at a valid position near the player.
    /// </summary>
    private void SpawnEmergencyKit()
    {
        if (roadTilePositions.Length == 0)
        {
            Debug.LogWarning(
                "[PickupManager] No RoadTiles found! Emergency Kit cannot spawn. Please check roadTile tags and/or presence of roadTile prefabs in the Scene!");
            return;
        }

        var spawnPosition = player.transform.position + player.transform.forward * emergencyKitDistance;

        // Find the closest RoadTile position to the ideal spawn point
        var closestPosition = roadTilePositions
            .OrderBy(pos => (new Vector2(pos.x, pos.z) - new Vector2(spawnPosition.x, spawnPosition.z)).sqrMagnitude)
            .First();

        // Set the correct height
        closestPosition.y = spawner.GetSpawnPosition().y;

        // Spawn the emergency kit at the closest RoadTile's position
        Instantiate(emergencyKitPrefab, closestPosition, Quaternion.identity);
        Debug.Log("[PickupManager] Emergency Kit spawned!");
    }

    /// <summary>
    ///     Spawns a pickup of the specified type and adds it to the active pickups collection.
    /// </summary>
    /// <param name="type">The type of pickup to spawn</param>
    /// <param name="prefab">The prefab to instantiate</param>
    private void SpawnPickup(string type, GameObject prefab)
    {
        var pickup = Instantiate(prefab, spawner.GetSpawnPosition(), Quaternion.identity);
        activePickups[type] = pickup;

        // Add timer component to handle expiration
        var timer = pickup.AddComponent<PickupTimer>();
        timer.SetExpirationTime(expirationTime);
    }

    /// <summary>
    ///     Gets the prefab for a specified pickup type.
    /// </summary>
    /// <param name="type">The type of pickup</param>
    /// <returns>The prefab GameObject for the specified type, or null if not found</returns>
    private GameObject GetPrefabForType(string type)
    {
        return type switch
        {
            "TShirt" => tShirtPrefab,
            "WinterCoat" => coatPrefab,
            "HotChocolate" => hotChocolatePrefab,
            "Lemonade" => lemonadePrefab,
            _ => null
        };
    }
}