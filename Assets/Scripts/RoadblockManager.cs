using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadblockManager : MonoBehaviour
{
    [SerializeField] private GameObject barrierPrefab; // Prefab to spawn
    [SerializeField] private float spawnInterval = 10f; // Time before repositioning
    [SerializeField] private int numberOfRoadblocks = 3; // How many roadblocks to spawn

    private List<Transform> barrierPositions = new List<Transform>(); // Store all intersection positions
    private List<GameObject> activeBarriers = new List<GameObject>(); // Store spawned roadblocks

    private void Start()
    {
        // Find all barrierPositions in the scene
        GameObject[] barrierPositionObjects = GameObject.FindGameObjectsWithTag("BarrierPosition");
        foreach (GameObject obj in barrierPositionObjects)
        {
            barrierPositions.Add(obj.transform);
        }

        if (barrierPositions.Count == 0)
        {
            Debug.LogError("No barrierPositions found! Make sure you tag them correctly.");
            return;
        }

        // Start the barrier placement cycle
        StartCoroutine(SpawnBarriers());
    }

    private IEnumerator SpawnBarriers()
    {
        while (true)
        {
            // Clear existing roadblocks
            foreach (GameObject barrier in activeBarriers)
            {
                Destroy(barrier);
            }
            activeBarriers.Clear();

            // Select random barrierPositions for roadblocks
            HashSet<int> chosenIndexes = new HashSet<int>();
            while (chosenIndexes.Count < numberOfRoadblocks && chosenIndexes.Count < barrierPositions.Count)
            {
                int randomIndex = Random.Range(0, barrierPositions.Count);
                if (!chosenIndexes.Contains(randomIndex))
                {
                    chosenIndexes.Add(randomIndex);
                }
            }

            // Spawn roadblocks at selected barrierPositions
            foreach (int index in chosenIndexes)
            {
                GameObject barrier = Instantiate(barrierPrefab, barrierPositions[index].position, Quaternion.identity);
                activeBarriers.Add(barrier);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
