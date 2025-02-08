using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TemperatureBar : MonoBehaviour
{
    [Header("UI Components")]
    public RectTransform coldFill;  // Left Side (Expands as it gets colder)
    public RectTransform normalFill; // Middle (Safe Zone)
    public RectTransform hotFill;   // Right Side (Expands as it gets hotter)
    public TextMeshProUGUI statusText;

    [Header("Temperature Settings")]
    public float minTemperature = -100f;
    public float maxTemperature = 100f;
    private float neutralRange = 20f; // Defines the "safe zone"

    private PlayerTemperature playerTemp;
    private float barWidth = 400f; // The total width of the bar

    void Start()
    {
        playerTemp = FindObjectOfType<PlayerTemperature>();
        if (playerTemp == null)
        {
            Debug.LogError("PlayerTemperature component not found!");
            return;
        }
        UpdateTemperatureBar();
    }

    void Update()
    {
        if (playerTemp != null)
        {
            UpdateTemperatureBar();
        }
    }

    void UpdateTemperatureBar()
    {
        float temperature = playerTemp.bodyTemperature;

        // Calculate proportions based on temperature
        float coldRatio = Mathf.Clamp01(Mathf.InverseLerp(0, minTemperature, temperature));
        float hotRatio = Mathf.Clamp01(Mathf.InverseLerp(0, maxTemperature, temperature));
        float normalRatio = 1f - (coldRatio + hotRatio);

        // Set width of UI elements
        coldFill.sizeDelta = new Vector2(barWidth * coldRatio, coldFill.sizeDelta.y);
        hotFill.sizeDelta = new Vector2(barWidth * hotRatio, hotFill.sizeDelta.y);
        normalFill.sizeDelta = new Vector2(barWidth * normalRatio, normalFill.sizeDelta.y);

        // Update Status Text
        UpdateStatusText(temperature);
    }

    void UpdateStatusText(float temperature)
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