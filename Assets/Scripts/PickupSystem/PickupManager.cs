using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField] private GameObject tShirtPrefab;
    [SerializeField] private GameObject coatPrefab;
    [SerializeField] private GameObject hotChocolatePrefab;
    [SerializeField] private GameObject lemonadePrefab;
    [SerializeField] private GameObject emergencyKitPrefab;

    [SerializeField] private float expirationTime = 30f;
    [SerializeField] private float emergencyKitDistance = 60f;
    [SerializeField] private float spawnHeight = 2f;

    private GameObject player;
    private PlayerTemperature playerTemperature;
    private PickupSpawner spawner;
    private Dictionary<string, GameObject> activePickups = new();
    private bool emergencyKitSpawned;
    private Vector3[] roadTilePositions;

    private void Start()
    {
        InitializeComponents();
        spawner = new PickupSpawner(spawnHeight);
        SpawnInitialPickups();

        // Cache all RoadTile positions at the start
        roadTilePositions = GameObject.FindGameObjectsWithTag("RoadTile")
            .Select(tile => tile.transform.position)
            .ToArray();
    }

    private void InitializeComponents()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found");
            enabled = false;
            return;
        }

        playerTemperature = player.GetComponent<PlayerTemperature>();
    }

    private void SpawnInitialPickups()
    {
        SpawnPickup("TShirt", tShirtPrefab);
        SpawnPickup("WinterCoat", coatPrefab);
        SpawnPickup("HotChocolate", hotChocolatePrefab);
        SpawnPickup("Lemonade", lemonadePrefab);
    }

    private void Update()
    {
        HandleExpiredPickups();
        CheckEmergencyKitSpawn();
    }

    private void HandleExpiredPickups()
    {
        var expiredPickups = new Dictionary<string, GameObject>(activePickups);
        foreach (var pickup in expiredPickups)
            if (IsPickupExpired(pickup.Value))
                RemoveAndRespawnPickup(pickup.Key);
    }

    private bool IsPickupExpired(GameObject pickup)
    {
        if (pickup == null) return true;
        var timer = pickup.GetComponent<PickupTimer>();
        return timer != null && timer.IsExpired();
    }

    private void RemoveAndRespawnPickup(string type)
    {
        if (activePickups.TryGetValue(type, out var pickup))
        {
            if (pickup != null) Destroy(pickup);
            activePickups.Remove(type);
            SpawnPickup(type, GetPrefabForType(type));
        }
    }

    private void CheckEmergencyKitSpawn()
    {
        if (emergencyKitSpawned) return;

        var temp = playerTemperature.bodyTemperature;
        if (temp < playerTemperature.minTemperature * 0.95f ||
            temp > playerTemperature.maxTemperature * 0.95f)
        {
            SpawnEmergencyKit();
            emergencyKitSpawned = true;
        }
    }

    private void SpawnEmergencyKit()
    {
         if (roadTilePositions.Length == 0)
        {
            Debug.LogWarning("No RoadTiles found! Emergency Kit not be able to spawn. Please check roatTile tags and/or presence of roadTile prefabs in the Scene!");
            return;
        }

        var spawnPosition = player.transform.position + player.transform.forward * emergencyKitDistance;
        
        // Find the closest RoadTile position
        Vector3 closestPosition = roadTilePositions
            .OrderBy(pos => (new Vector2(pos.x, pos.z) - new Vector2(spawnPosition.x, spawnPosition.z)).sqrMagnitude)
            .First();
        // Get a Y position above the road
        closestPosition.y = spawner.GetSpawnPosition().y;
        // Spawn the emergency kit at the closest RoadTile's position
        Instantiate(emergencyKitPrefab, closestPosition, Quaternion.identity);
        Debug.Log("Emergency Kit spawned!");
    }

    private void SpawnPickup(string type, GameObject prefab)
    {
        var pickup = Instantiate(prefab, spawner.GetSpawnPosition(), Quaternion.identity);
        activePickups[type] = pickup;

        var timer = pickup.AddComponent<PickupTimer>();
        timer.SetExpirationTime(expirationTime);
    }

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