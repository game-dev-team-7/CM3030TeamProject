using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private GameObject customerPrefab; // Prefab to spawn
    private GameObject currentCustomer; // Reference to the current customer
    private float spawnYPosition;

    private GameObject streets; // Streets object
    private readonly List<Renderer> tarmacRenderers = new(); // Tarmac is the street unit with Mesh Renderer

    private readonly float baseTimePerUnitDistance = 0.04f;
    private float timer; // Timer for the current customer
    private TextMeshProUGUI timerText; //Reference the Timer UI

    private int score;
    private int streak;
    private TextMeshProUGUI scoreText; // Reference to the Score UI
    private TextMeshProUGUI streakText; // Reference to the Streak UI


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("Player object not found");

        streets = GameObject.FindGameObjectWithTag("Streets");
        if (streets != null)
            tarmacRenderers.AddRange(streets.GetComponentsInChildren<Renderer>());
        else
            Debug.LogError("Streets object not found");

        var ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null)
        {
            var groundRenderer = ground.GetComponent<Renderer>();
            spawnYPosition = groundRenderer.bounds.max.y + 9.8f;
        }
        else
        {
            Debug.LogError("Ground object not found");
        }

        var timerObject = GameObject.FindGameObjectWithTag("CustomerTimer");
        if (timerObject != null)
            timerText = timerObject.GetComponent<TextMeshProUGUI>();
        else
            Debug.LogError("No UI object found with tag 'CustomerTimer'.");

        var scoreObject = GameObject.FindGameObjectWithTag("Score");
        if (scoreObject != null)
            scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        else
            Debug.LogError("No UI object found with tag 'Score'.");

        var streakObject = GameObject.FindGameObjectWithTag("Streak");
        if (streakObject != null)
            streakText = streakObject.GetComponent<TextMeshProUGUI>();
        else
            Debug.LogError("No UI object found with tag 'Streak'.");

        SpawnCustomer(); // Spawn first customer
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer).ToString();
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

        var chosenTarmacRenderer = tarmacRenderers[Random.Range(0, tarmacRenderers.Count)];
        var spawnPosition = GetRandomPointInBounds(chosenTarmacRenderer.bounds);
        spawnPosition.y = spawnYPosition;

        currentCustomer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);

        ResetTimer();
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
        var expirationTime = Vector3.Distance(currentCustomer.transform.position, player.transform.position) *
                             baseTimePerUnitDistance;
        timer = expirationTime;
        timerText.text = Mathf.CeilToInt(timer).ToString();
    }

    private void FailDelivery()
    {
        streak = 0;

        if (streakText != null)
        {
            streakText.text = "Task Failed";
            Invoke(nameof(ResetStreakText), 1.5f);
        }

        Debug.Log("Timer expired! Respawning customer.");
        SpawnCustomer();
    }

    // Handle result of Player-Customer collision
    public void CompleteDelivery()
    {
        streak++;

        if (streakText != null)
        {
            streakText.text = "+" + streak;
            Invoke(nameof(ResetStreakText), 1.5f);
        }

        score += streak;

        if (scoreText != null) scoreText.text = score.ToString();

        Debug.Log("Player interacted! Score: " + score);
        SpawnCustomer();
    }

    private void ResetStreakText()
    {
        streakText.text = "";
    }
}