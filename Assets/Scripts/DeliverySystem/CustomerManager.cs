using System.Collections;
using UnityEngine;
using UnityEngine.AI;
//Required for NavMesh

/// <summary>
///     Manages the customer delivery gameplay loop including spawning customers,
///     tracking delivery time, and handling success/failure conditions.
/// </summary>
public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private float spawnHeightOffset = 2f;
    [SerializeField] private float baseTimePerUnitDistance = 0.04f;
    [SerializeField] private GameObject navMeshObject;
    private DeliveryTimer deliveryTimer;
    private GameObject player;
    private ScoreManager scoreManager;

    private CustomerSpawner spawner;
    private UIManager uiManager;

    /// <summary>
    ///     Initializes component references and creates helper objects.
    /// </summary>
    private void Awake()
    {
        if (!TryInitialize())
        {
            enabled = false;
            return;
        }

        spawner = new CustomerSpawner(customerPrefab, spawnHeightOffset);
        deliveryTimer = new DeliveryTimer();

        scoreManager = GetComponent<ScoreManager>();
        uiManager = GetComponent<UIManager>();
    }

    /// <summary>
    ///     Begins the initial delivery once the scene is loaded.
    /// </summary>
    private void Start()
    {
        StartNewDelivery();
    }

    /// <summary>
    ///     Updates the delivery timer and checks for expiration.
    /// </summary>
    private void Update()
    {
        if (!enabled) return;

        if (GameObject.FindGameObjectWithTag("Customer") == null) return;

        deliveryTimer.UpdateTimer();
        uiManager.UpdateTimer(deliveryTimer.RemainingTime);

        if (deliveryTimer.IsTimeExpired()) FailDelivery();
    }

    /// <summary>
    ///     Attempts to find required game objects and validates initialization.
    /// </summary>
    /// <returns>True if initialization was successful, false otherwise</returns>
    private bool TryInitialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        return player != null;
    }

    /// <summary>
    ///     Spawns a new customer and starts the delivery timer.
    /// </summary>
    private void StartNewDelivery()
    {
        spawner.SpawnCustomer();
        var deliveryTime = CalculateDeliveryTime();
        deliveryTimer.StartTimer(deliveryTime);
        uiManager.UpdateTimer(deliveryTime);
    }

    /// <summary>
    ///     Calculates the delivery time based on the NavMesh path distance between player and customer.
    /// </summary>
    /// <returns>The calculated time limit for the delivery in seconds</returns>
    private float CalculateDeliveryTime()
    {
        //return Vector3.Distance(spawner.GetCurrentCustomerPosition(), player.transform.position) *
        //       baseTimePerUnitDistance;
        var path = new NavMeshPath();
        var start = player.transform.position;
        var end = spawner.GetCurrentCustomerPosition();
        NavMeshHit hit;

        // Ensure start position is on the NavMesh
        if (!NavMesh.SamplePosition(start, out hit, 80f, NavMesh.AllAreas))
        {
            Debug.LogError("[CustomerManager] Player position is not on NavMesh!");
            return float.MaxValue;
        }

        start = hit.position;

        // Ensure end position is on the NavMesh
        if (!NavMesh.SamplePosition(end, out hit, 10f, NavMesh.AllAreas))
        {
            Debug.LogError("[CustomerManager] Customer position is not on NavMesh!");
            return float.MaxValue;
        }

        end = hit.position;

        // Try calculating the path
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            Debug.Log("[CustomerManager] Path successfully found!");
            var pathDistance = GetPathLength(path);
            return pathDistance * baseTimePerUnitDistance;
        }

        Debug.LogWarning("[CustomerManager] Path not found or incomplete.");
        return float.MaxValue;
    }

    /// <summary>
    ///     Calculates the total length of a NavMesh path by summing distances between its corners.
    /// </summary>
    /// <param name="path">The NavMesh path to measure</param>
    /// <returns>The total length of the path</returns>
    private float GetPathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
        {
            Debug.LogWarning("[CustomerManager] Path has insufficient corners.");
            return float.MaxValue;
        }

        var distance = 0f;
        for (var i = 1; i < path.corners.Length; i++)
            distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);

        return distance;
    }

    /// <summary>
    ///     Handles delivery failure logic, including UI updates and score penalties.
    /// </summary>
    private void FailDelivery()
    {
        spawner.DestroyCurrentCustomer();
        scoreManager.ResetStreak();
        uiManager.ShowFailureMessage();
        uiManager.ResetTimer();
        StartCoroutine(StartNewDeliveryWithDelay());
    }

    /// <summary>
    ///     Handles successful delivery logic, including score updates and UI notifications.
    ///     Called by CustomerCollision when player reaches the customer.
    /// </summary>
    public void CompleteDelivery()
    {
        spawner.DestroyCurrentCustomer();
        scoreManager.IncrementScore();
        uiManager.ShowSuccessMessage(scoreManager.CurrentStreak);
        uiManager.UpdateScore(scoreManager.CurrentScore);
        uiManager.ResetTimer();
        StartCoroutine(StartNewDeliveryWithDelay());
    }

    /// <summary>
    ///     Waits for a short period before starting a new delivery.
    /// </summary>
    /// <returns>IEnumerator for the coroutine</returns>
    private IEnumerator StartNewDeliveryWithDelay()
    {
        yield return new WaitForSeconds(2f);
        StartNewDelivery();
    }

    /// <summary>
    ///     Enables the customer manager functionality.
    /// </summary>
    public void EnableManager()
    {
        enabled = true;
        Debug.Log("CustomerManager has been enabled.");
    }

    /// <summary>
    ///     Disables the customer manager functionality.
    /// </summary>
    public void DisableManager()
    {
        enabled = false;
        Debug.Log("CustomerManager has been disabled.");
    }
}