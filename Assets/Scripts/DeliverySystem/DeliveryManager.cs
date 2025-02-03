using UnityEngine;
using System.Collections.Generic;

namespace DeliverySystem
{
    public class DeliveryManager : MonoBehaviour
    {
        public static DeliveryManager Instance { get; private set; }

        [Header("References")] public Transform player;
        public GameObject deliveryTaskPrefab;
        public List<Transform> spawnPoints = new();

        [Header("Settings")] public float baseTimePerUnitDistance = 0.09f;
        public float spawnDelay = 2f;

        [Header("State")] public int score;
        public int streak;
        public DeliveryTask currentTask;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            StartNewDeliveryCycle();
        }

        private void StartNewDeliveryCycle()
        {
            StartCoroutine(SpawnNewTaskWithDelay());
        }

        private IEnumerator<WaitForSeconds> SpawnNewTaskWithDelay()
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnNewTask();
        }

        private void SpawnNewTask()
        {
            if (spawnPoints.Count == 0) return;

            var randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            var taskGo = Instantiate(deliveryTaskPrefab, randomPoint.position, Quaternion.identity);
            currentTask = taskGo.GetComponent<DeliveryTask>();
            currentTask.Initialize(player);
        }

        public void CompleteDelivery()
        {
            streak++;
            score += streak; // 1 point base + streak bonus
            Destroy(currentTask.gameObject);
            StartNewDeliveryCycle();
            UIManager.instance.UpdateUI();
        }

        public void FailDelivery()
        {   
            streak = 0;
            Destroy(currentTask.gameObject);
            StartNewDeliveryCycle();
            UIManager.instance.UpdateUI();
        }
    }
}