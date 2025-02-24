using UnityEngine;
using System.Collections;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private float spawnHeightOffset = 2f;
    [SerializeField] private float baseTimePerUnitDistance = 0.04f;

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
        return Vector3.Distance(spawner.GetCurrentCustomerPosition(), player.transform.position) *
               baseTimePerUnitDistance;
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
}