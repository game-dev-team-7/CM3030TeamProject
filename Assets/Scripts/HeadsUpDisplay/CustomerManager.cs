using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab; // Prefab to spawn
    private GameObject currentCustomer; // Reference to the current customer

    [SerializeField] private GameObject streets; // Streets object
    private List<Renderer> tarmacRenderers = new(); // Tarmac is the smallest unit of streets with Mesh Renderer

    private float countdownTime = 120f; // Timer duration
    private float currentCountdown; // Timer
    private TextMeshProUGUI timerText; //Reference the Timer UI

    private int scoreIncrement = 10; // Points per interaction
    private TextMeshProUGUI scoreText; // Reference to the Score UI
    private int playerScore = 0;

    private void Start()
    {
        if (streets != null)
            tarmacRenderers.AddRange(streets.GetComponentsInChildren<Renderer>());
        else
            Debug.LogError("Streets object not assigned!");


        var scoreObject = GameObject.FindGameObjectWithTag("Score");
        if (scoreObject != null)
            scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        else
            Debug.LogError("No UI object found with tag 'Score'.");

        var timerObject = GameObject.FindGameObjectWithTag("CustomerTimer");
        if (timerObject != null)
            timerText = timerObject.GetComponent<TextMeshProUGUI>();
        else
            Debug.LogError("No UI object found with tag 'CustomerTimer'.");

        SpawnCustomer(); // Spawn first customer
    }

    private void Update()
    {
        if (currentCountdown > 0)
        {
            currentCountdown -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(currentCountdown).ToString();
        }
        else
        {
            HandleTimerExpiry();
        }
    }

    private void SpawnCustomer()
    {
        if (tarmacRenderers.Count == 0 || customerPrefab == null) return;

        ResetTimer();

        if (currentCustomer != null) Destroy(currentCustomer);

        var chosenTarmacRenderer = tarmacRenderers[Random.Range(0, tarmacRenderers.Count)];
        var spawnPosition = GetRandomPointInBounds(chosenTarmacRenderer.bounds);
        spawnPosition.y = chosenTarmacRenderer.bounds.max.y + 7.8f; // Slightly above the surface

        currentCustomer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void ResetTimer()
    {
        currentCountdown = countdownTime;
        timerText.text = Mathf.CeilToInt(currentCountdown).ToString();
    }

    private void HandleTimerExpiry()
    {
        Debug.Log("Timer expired! Respawning customer.");
        SpawnCustomer();
    }

    // Handle result of Player-Customer collision
    public void HandlePlayerInteraction()
    {
        playerScore += scoreIncrement;

        if (scoreText != null) scoreText.text = playerScore.ToString();

        Debug.Log("Player interacted! Score: " + playerScore);
        SpawnCustomer();
    }
}