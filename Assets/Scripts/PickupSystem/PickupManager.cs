using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField] private GameObject tShirtPrefab;
    [SerializeField] private GameObject coatPrefab;
    [SerializeField] private GameObject hotChocolatePrefab;
    [SerializeField] private GameObject lemonadePrefab;

    private readonly List<GameObject> plazas = new();
    private readonly List<Renderer> tarmacRenderers = new();
    private float spawnYPosition;

    private readonly float expirationTime = 20f;
    private readonly Dictionary<string, GameObject> activePickups = new();

    private void Start()
    {
        // spawn should be slightly above the ground
        var ground = GameObject.FindGameObjectWithTag("Ground");
        var groundRenderer = ground.GetComponent<Renderer>();
        spawnYPosition = groundRenderer.bounds.max.y + 2f;

        // Get all game objects with tag "plaza" and add them to plazas list
        plazas.AddRange(GameObject.FindGameObjectsWithTag("Plaza"));

        // Get all renderers from plaza objects
        plazas.ForEach(plaza => tarmacRenderers.AddRange(plaza.GetComponentsInChildren<Renderer>()));

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