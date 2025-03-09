using TMPro;
using UnityEngine;

/// <summary>
///     Manages the temperature UI bar that visualizes player temperature status.
///     This component handles the UI representation of the player's current body temperature.
/// </summary>
public class TemperatureBar : MonoBehaviour
{
    [Header("UI Components")] public RectTransform coldFill; // Left Side (Expands as it gets colder)

    public RectTransform normalFill; // Middle (Safe Zone)
    public RectTransform hotFill; // Right Side (Expands as it gets hotter)
    public TextMeshProUGUI statusText;

    [Header("Temperature Settings")]
    public float minTemperature = -100f; // Minimum possible temperature (corresponds to full cold bar)

    public float maxTemperature = 100f; // Maximum possible temperature (corresponds to full hot bar)
    private readonly float barWidth = 400f; // The total width of the bar in pixels
    private readonly float neutralRange = 20f; // Defines the "safe zone" temperature range

    private PlayerTemperature playerTemp;

    /// <summary>
    ///     Initializes the temperature bar by finding the player temperature component.
    /// </summary>
    private void Start()
    {
        playerTemp = FindObjectOfType<PlayerTemperature>();
        if (playerTemp == null)
        {
            Debug.LogError("PlayerTemperature component not found!");
            return;
        }

        UpdateTemperatureBar();
    }

    /// <summary>
    ///     Updates the temperature bar every frame to reflect the current player temperature.
    /// </summary>
    private void Update()
    {
        if (playerTemp != null) UpdateTemperatureBar();
    }

    /// <summary>
    ///     Updates the temperature bar UI elements based on the current temperature value.
    ///     Calculates the proportions of cold, normal, and hot sections.
    /// </summary>
    private void UpdateTemperatureBar()
    {
        var temperature = playerTemp.bodyTemperature;

        // Calculate proportions based on temperature
        var coldRatio = Mathf.Clamp01(Mathf.InverseLerp(0, minTemperature, temperature));
        var hotRatio = Mathf.Clamp01(Mathf.InverseLerp(0, maxTemperature, temperature));
        var normalRatio = 1f - (coldRatio + hotRatio);

        // Set width of UI elements
        coldFill.sizeDelta = new Vector2(barWidth * coldRatio, coldFill.sizeDelta.y);
        hotFill.sizeDelta = new Vector2(barWidth * hotRatio, hotFill.sizeDelta.y);
        normalFill.sizeDelta = new Vector2(barWidth * normalRatio, normalFill.sizeDelta.y);

        // Update Status Text
        UpdateStatusText(temperature);
    }

    /// <summary>
    ///     Updates the status text and color based on the current temperature value.
    ///     Provides visual feedback to the player about their temperature status.
    /// </summary>
    /// <param name="temperature">The current player temperature value</param>
    private void UpdateStatusText(float temperature)
    {
        if (statusText == null)
        {
            Debug.LogError("Status Text is not assigned!");
            return;
        }

        statusText.gameObject.SetActive(true); // Force text to stay visible
        statusText.transform.SetAsLastSibling(); // Move text on top

        if (temperature <= minTemperature * 0.5f)
        {
            statusText.text = "❄️ FREEZING!";
            statusText.color = Color.cyan;
        }
        else if (temperature <= -neutralRange)
        {
            statusText.text = "COLD";
            statusText.color = Color.Lerp(Color.green, Color.cyan, 0.5f);
        }
        else if (temperature >= maxTemperature * 0.5f)
        {
            statusText.text = "🔥 OVERHEATED!";
            statusText.color = Color.red;
        }
        else if (temperature >= neutralRange)
        {
            statusText.text = "HOT";
            statusText.color = Color.Lerp(Color.green, Color.red, 0.5f);
        }
        else
        {
            statusText.text = "✅ NORMAL";
            statusText.color = Color.green;
            statusText.transform.SetAsLastSibling(); // Force it to be on top
        }
    }
}