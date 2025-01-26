// TemperatureBar.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TemperatureBar : MonoBehaviour
{
    [Header("UI Components")]
    public Image fillImage; // The fill Image (TempBar_Fill)
    public TextMeshProUGUI statusText; // The status Text (TempBar_StatusText)

    [Header("Color Settings")]
    public Color coldColor = new Color(0f, 0.5f, 1f, 0.5f); // Blueish
    public Color normalColor = new Color(1f, 1f, 0f, 0.5f); // Yellow
    public Color hotColor = new Color(1f, 0f, 0f, 0.5f); // Red

    [Header("Temperature Thresholds")]
    public float coldThreshold = -50f;
    public float hotThreshold = 50f;

    [Header("Status Text")]
    public string coldStatus = "Cold";
    public string normalStatus = "Normal";
    public string hotStatus = "Hot";

    private PlayerTemperature playerTemp;

    void Start()
    {
        // Find the PlayerTemperature component
        playerTemp = FindObjectOfType<PlayerTemperature>();
        if (playerTemp == null)
        {
            Debug.LogError("PlayerTemperature component not found in the scene.");
        }

        // Initialize UI
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
        // Normalize temperature to 0-1 for the fill amount
        float normalizedTemp = Mathf.InverseLerp(playerTemp.minTemperature, playerTemp.maxTemperature, playerTemp.bodyTemperature);
        fillImage.fillAmount = normalizedTemp;

        // Determine the current status
        string currentStatus;
        Color currentColor;

        if (playerTemp.bodyTemperature <= coldThreshold)
        {
            currentStatus = coldStatus;
            currentColor = coldColor;
        }
        else if (playerTemp.bodyTemperature >= hotThreshold)
        {
            currentStatus = hotStatus;
            currentColor = hotColor;
        }
        else
        {
            currentStatus = normalStatus;
            currentColor = normalColor;
        }

        // Update fill color smoothly
        fillImage.color = Color.Lerp(fillImage.color, currentColor, Time.deltaTime * 5f);

        // Update status text
        statusText.text = currentStatus;
    }
}
