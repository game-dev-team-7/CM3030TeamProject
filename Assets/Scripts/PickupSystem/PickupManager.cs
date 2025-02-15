using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    private GameObject player;
    private float minTemp;
    private float maxTemp;

    [SerializeField] private GameObject tShirtPrefab;
    [SerializeField] private GameObject coatPrefab;
    [SerializeField] private GameObject hotChocolatePrefab;
    [SerializeField] private GameObject lemonadePrefab;

    [SerializeField] private GameObject emergencyKitPrefab;
    private bool emergencyKitSpawned = false;

    private readonly List<Renderer> tarmacRenderers = new();
    private float spawnYPosition;

    private readonly float expirationTime = 20f;
    private readonly Dictionary<string, GameObject> activePickups = new();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("Player object not found");
        maxTemp = player.GetComponent<PlayerTemperature>().maxTemperature;
        minTemp = player.GetComponent<PlayerTemperature>().minTemperature;

        // spawn should be slightly above the ground
        var ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null)
        {
            var groundRenderer = ground.GetComponent<Renderer>();
            spawnYPosition = groundRenderer.bounds.max.y + 2f;
        }
        else
        {
            Debug.LogError("Ground object not found");
        }

        // Determine where to spawn the pickups
        var innerBlocks = GameObject.FindGameObjectWithTag("InnerBlocks");
        if (innerBlocks != null)
            // Find all renderers from child objects with the "Plaza" tag within innerBlocks
            tarmacRenderers.AddRange(innerBlocks.GetComponentsInChildren<Transform>()
                .Where(t => t.CompareTag("Plaza"))
                .SelectMany(t => t.GetComponentsInChildren<Renderer>()));
        else
            Debug.LogError("InnerBlocks object not found");

        // Spawn initial pickups
        SpawnPickup("TShirt", tShirtPrefab);
        SpawnPickup("Coat", coatPrefab);
        SpawnPickup("HotChocolate", hotChocolatePrefab);
        SpawnPickup("Lemonade", lemonadePrefab);
    }

    private void Update()
    {
        // Check for expired pickups
        foreach (var pickup in new Dictionary<string, GameObject>(activePickups))
            if (pickup.Value == null)
            {
                activePickups.Remove(pickup.Key);
                SpawnPickup(pickup.Key, GetPrefabForType(pickup.Key));
            }
            else
            {
                var timer = pickup.Value.GetComponent<PickupTimer>();
                if (timer != null && timer.IsExpired())
                {
                    Destroy(pickup.Value);
                    activePickups.Remove(pickup.Key);
                    SpawnPickup(pickup.Key, GetPrefabForType(pickup.Key));
                }
            }

        var temp = player.GetComponent<PlayerTemperature>().bodyTemperature;

        if ((temp < minTemp * 0.95f || temp > maxTemp * 0.95f) && !emergencyKitSpawned)
        {
            SpawnEmergencyKit();
            emergencyKitSpawned = true;
        }
    }

    private void SpawnEmergencyKit()
    {
        var playerPosition = player.transform.position;
        var spawnPosition = playerPosition + player.transform.forward * 60f; // Spawn in front of the player
        spawnPosition.y = spawnYPosition;

        Instantiate(emergencyKitPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Emergency Kit spawned!");
    }

    private void SpawnPickup(string type, GameObject prefab)
    {
        var chosenTarmacRenderer = tarmacRenderers[Random.Range(0, tarmacRenderers.Count)];
        var spawnPosition = GetRandomPointInBounds(chosenTarmacRenderer.bounds);
        spawnPosition.y = spawnYPosition;

        var pickup = Instantiate(prefab, spawnPosition, Quaternion.identity);
        activePickups[type] = pickup;

        // Add PickupTimer component to the spawned pickup
        var timer = pickup.AddComponent<PickupTimer>();
        timer.SetExpirationTime(expirationTime);
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private GameObject GetPrefabForType(string type)
    {
        switch (type)
        {
            case "TShirt": return tShirtPrefab;
            case "Coat": return coatPrefab;
            case "HotChocolate": return hotChocolatePrefab;
            case "Lemonade": return lemonadePrefab;
            default: return null;
        }
    }
}