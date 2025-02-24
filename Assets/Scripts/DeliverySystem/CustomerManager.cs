using System.Collections.Generic;
using UnityEngine;

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

        deliveryTimer.UpdateTimer();
        uiManager.UpdateTimer(Mathf.CeilToInt(deliveryTimer.RemainingTime));

        if (deliveryTimer.IsTimeExpired()) FailDelivery();
    }

    private void StartNewDelivery()
    {
        spawner.SpawnCustomer();
        var deliveryTime = CalculateDeliveryTime();
        deliveryTimer.StartTimer(deliveryTime);
        uiManager.UpdateTimer(Mathf.CeilToInt(deliveryTime));
    }

    private float CalculateDeliveryTime()
    {
        return Vector3.Distance(spawner.GetCurrentCustomerPosition(), player.transform.position) *
               baseTimePerUnitDistance;
    }

    private void FailDelivery()
    {
        scoreManager.ResetStreak();
        uiManager.ShowFailureMessage();
        StartNewDelivery();
    }

    public void CompleteDelivery()
    {
        scoreManager.IncrementScore();
        uiManager.ShowSuccessMessage(scoreManager.CurrentStreak);
        uiManager.UpdateScore(scoreManager.CurrentScore);
        StartNewDelivery();
    }
}