using UnityEngine;
using UnityEngine.AI; //Required for NavMesh
using System.Collections;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private float spawnHeightOffset = 2f;
    [SerializeField] private float baseTimePerUnitDistance = 0.04f;
    [SerializeField] private GameObject navMeshObject;

    private CustomerSpawner spawner;
    private DeliveryTimer deliveryTimer;
    private ScoreManager scoreManager;
    private UIManager uiManager;
    private GameObject player;

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

    private bool TryInitialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        return player != null;
    }

    private void Start()
    {
        StartNewDelivery();
    }

    private void Update()
    {
        if (!enabled) return;

        if (GameObject.FindGameObjectWithTag("Customer") == null) return;

        deliveryTimer.UpdateTimer();
        uiManager.UpdateTimer(deliveryTimer.RemainingTime);

        if (deliveryTimer.IsTimeExpired()) FailDelivery();
    }

    private void StartNewDelivery()
    {
        spawner.SpawnCustomer();
        var deliveryTime = CalculateDeliveryTime();
        deliveryTimer.StartTimer(deliveryTime);
        uiManager.UpdateTimer(deliveryTime);
    }

    private float CalculateDeliveryTime()
    {
        //return Vector3.Distance(spawner.GetCurrentCustomerPosition(), player.transform.position) *
        //       baseTimePerUnitDistance;
        NavMeshPath path = new NavMeshPath();
        Vector3 start = player.transform.position;
        Vector3 end = spawner.GetCurrentCustomerPosition();
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
            float pathDistance = GetPathLength(path);
            return pathDistance * baseTimePerUnitDistance;
        }

        Debug.LogWarning("[CustomerManager] Path not found or incomplete.");
        return float.MaxValue;
    }

    private float GetPathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
        {
            Debug.LogWarning("[CustomerManager] Path has insufficient corners.");
            return float.MaxValue;
        }

        float distance = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return distance;
    }

    private void FailDelivery()
    {
        spawner.DestroyCurrentCustomer();
        scoreManager.ResetStreak();
        uiManager.ShowFailureMessage();
        uiManager.ResetTimer();
        StartCoroutine(StartNewDeliveryWithDelay());
    }

    public void CompleteDelivery()
    {
        spawner.DestroyCurrentCustomer();
        scoreManager.IncrementScore();
        uiManager.ShowSuccessMessage(scoreManager.CurrentStreak);
        uiManager.UpdateScore(scoreManager.CurrentScore);
        uiManager.ResetTimer();
        StartCoroutine(StartNewDeliveryWithDelay());
    }

    private IEnumerator StartNewDeliveryWithDelay()
    {
        yield return new WaitForSeconds(2f);
        StartNewDelivery();
    }

    public void EnableManager()
    {
        enabled = true;
        Debug.Log("CustomerManager has been enabled.");
    }

    public void DisableManager()
    {
        enabled = false;
        Debug.Log("CustomerManager has been disabled.");
    }
}