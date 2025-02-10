using UnityEngine;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab; // Prefab to spawn
    [SerializeField] private float countdownTime = 30f; // Timer duration
    [SerializeField] private int scoreIncrement = 10; // Points per interaction

    private GameObject[] customerLocations; // Array of possible spawn locations
    private GameObject currentCustomer; // Reference to the current customer
    private float currentCountdown; // Timer
    private TextMeshProUGUI scoreText; // Reference to the Score UI
    private TextMeshProUGUI timerText; //Reference the Timer UI
    private int playerScore = 0;

    private void Start()
    {
        customerLocations = GameObject.FindGameObjectsWithTag("CustomerLocation");

        if (customerLocations.Length == 0)
        {
            Debug.LogError("No objects found with tag 'CustomerLocation'.");
            return;
        }

        GameObject scoreObject = GameObject.FindGameObjectWithTag("Score");
        if (scoreObject != null)
        {
            scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("No UI object found with tag 'Score'.");
        }

        GameObject timerObject = GameObject.FindGameObjectWithTag("CustomerTimer");
        if (timerObject != null)
        {
            timerText = timerObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("No UI object found with tag 'CustomerTimer'.");
        }

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
        ResetTimer();
        
        if (customerLocations.Length == 0 || customerPrefab == null) return;

        Transform newLocation = customerLocations[Random.Range(0, customerLocations.Length)].transform;

        if (currentCustomer != null) Destroy(currentCustomer);

        currentCustomer = Instantiate(customerPrefab, newLocation.position, Quaternion.identity);

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

        if (scoreText != null)
        {
            scoreText.text = playerScore.ToString();
        }

        Debug.Log("Player interacted! Score: " + playerScore);
        SpawnCustomer();
    }
}
