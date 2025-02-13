using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject customerPrefab; // Prefab to spawn
    private GameObject currentCustomer; // Reference to the current customer
    private float heightAboveTarmac = 7.8f;

    [SerializeField] private GameObject streets; // Streets object
    private List<Renderer> tarmacRenderers = new(); // Tarmac is the street unit with Mesh Renderer

    private float expirationTime; // How long can the customer last
    private float baseTimePerUnitDistance = 0.05f;
    private float timer; // Timer for the current customer
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
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer).ToString();
        }
        else
        {
            HandleTimerExpiry();
        }
    }

    private void SpawnCustomer()
    {
        if (tarmacRenderers.Count == 0 || customerPrefab == null) return;

        if (currentCustomer != null) Destroy(currentCustomer);

        var chosenTarmacRenderer = tarmacRenderers[Random.Range(0, tarmacRenderers.Count)];
        var spawnPosition = GetRandomPointInBounds(chosenTarmacRenderer.bounds);
        spawnPosition.y = chosenTarmacRenderer.bounds.max.y + heightAboveTarmac; // Slightly above the surface

        currentCustomer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);

        ResetTimer();
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
        expirationTime = Vector3.Distance(currentCustomer.transform.position, player.transform.position) *
                         baseTimePerUnitDistance;
        timer = expirationTime;
        timerText.text = Mathf.CeilToInt(timer).ToString();
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