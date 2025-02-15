using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private float spawnHeightOffset = 2f;
    [SerializeField] private float baseTimePerUnitDistance = 0.04f;

    private GameObject player;
    private GameObject currentCustomer;
    private float spawnYPosition;
    private float timer;

    private readonly List<Renderer> tarmacRenderers = new();
    private ScoreManager scoreManager;
    private UIManager uiManager;

    private void Awake()
    {
        scoreManager = GetComponent<ScoreManager>();
        uiManager = GetComponent<UIManager>();

        if (!TryInitialize()) enabled = false;
    }

    private bool TryInitialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found");
            return false;
        }

        InitializeSpawnArea();
        return true;
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

    private void Start()
    {
        SpawnCustomer();
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            uiManager.UpdateTimer(Mathf.CeilToInt(timer));
        }
        else
        {
            FailDelivery();
        }
    }

    private void SpawnCustomer()
    {
        if (tarmacRenderers.Count == 0 || customerPrefab == null) return;

        if (currentCustomer != null) Destroy(currentCustomer);

        var spawnPosition = GetSpawnPosition();
        currentCustomer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);

        ResetTimer();
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

    private void ResetTimer()
    {
        timer = CalculateDeliveryTime();
        uiManager.UpdateTimer(Mathf.CeilToInt(timer));
    }

    private float CalculateDeliveryTime()
    {
        return Vector3.Distance(currentCustomer.transform.position, player.transform.position) *
               baseTimePerUnitDistance;
    }

    private void FailDelivery()
    {
        scoreManager.ResetStreak();
        uiManager.ShowFailureMessage();
        SpawnCustomer();
    }

    public void CompleteDelivery()
    {
        scoreManager.IncrementScore();
        uiManager.ShowSuccessMessage(scoreManager.CurrentStreak);
        uiManager.UpdateScore(scoreManager.CurrentScore);
        SpawnCustomer();
    }
}