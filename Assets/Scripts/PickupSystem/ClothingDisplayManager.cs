using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Manages the visual display of the player's current clothing item and provides feedback
///     about appropriate clothing for the current weather conditions.
/// </summary>
public class ClothingDisplayManager : MonoBehaviour
{
    [Header("References")] [Tooltip("Reference to the player temperature component")] [SerializeField]
    private PlayerTemperature playerTemp;

    [Tooltip("Reference to the weather manager")] [SerializeField]
    private WeatherManager weatherManager;

    [Tooltip("Reference to the player's pickup sound component")] [SerializeField]
    private PlayerPickupSounds playerPickupSounds;

    [Tooltip("Reference to the T-shirt model transform")] [SerializeField]
    private Transform tShirt;

    [Tooltip("Reference to the winter coat model transform")] [SerializeField]
    private Transform winterCoat;

    [Header("UI Elements")] [Tooltip("Text component for displaying clothing-related tips")] [SerializeField]
    private TextMeshProUGUI clothingTipText;

    [Header("Pulsing Effect Settings")] [Tooltip("Speed of the pulsing effect")] [SerializeField]
    private float pulseSpeed = 2f;

    [Tooltip("Maximum intensity of the pulsing effect")] [SerializeField]
    private float pulseIntensity = 0.5f;

    [Tooltip("Color of the outline when pulsing as a warning")] [SerializeField]
    private Color warningPulseColor = Color.red;

    [Header("Outline Components")] [Tooltip("Outline component for the T-shirt model")] [SerializeField]
    private Outline tShirtOutline;

    [Tooltip("Outline component for the winter coat model")] [SerializeField]
    private Outline winterCoatOutline;

    /// <summary>The player's current clothing type</summary>
    private ClothingType currentClothing = ClothingType.None;

    /// <summary>Flag indicating if the current clothing is inappropriate for the weather</summary>
    private bool isInappropriateClothing;

    /// <summary>
    ///     Initializes the component and validates required references.
    /// </summary>
    private void Start()
    {
        if (playerTemp == null)
        {
            Debug.LogError("[ClothingDisplayManager] PlayerTemperature script not found!");
            return;
        }

        if (weatherManager == null)
        {
            Debug.LogError("[ClothingDisplayManager] WeatherManager script not found!");
            return;
        }

        if (playerPickupSounds == null)
        {
            Debug.LogError("[ClothingDisplayManager] PlayerPickupSounds script not found!");
            return;
        }

        // Ensure text component is not null
        if (clothingTipText == null) Debug.LogError("[ClothingDisplayManager] Clothing tip text is not assigned!");
    }

    /// <summary>
    ///     Updates the clothing display each frame based on the player's current clothing.
    /// </summary>
    private void Update()
    {
        // Update current clothing
        currentClothing = playerTemp.currentClothing;

        // Check for inappropriate clothing
        CheckClothingAppropriate();

        // Display clothing and update UI
        DisplayCurrentClothing(currentClothing);

        // Handle Tab key for removing clothing
        HandleClothingRemoval();
    }

    /// <summary>
    ///     Checks if the current clothing is appropriate for the weather conditions.
    /// </summary>
    private void CheckClothingAppropriate()
    {
        var currentWeather = weatherManager.CurrentWeather;
        isInappropriateClothing = false;
        var tipMessage = "";

        switch (currentClothing)
        {
            case ClothingType.TShirt:
                if (currentWeather == WeatherType.Snowstorm)
                {
                    isInappropriateClothing = true;
                    tipMessage = "Put on a winter coat!";
                }

                break;
            case ClothingType.WinterCoat:
                if (currentWeather == WeatherType.Heatwave)
                {
                    isInappropriateClothing = true;
                    tipMessage = "Put on a t-shirt!";
                }

                break;
        }

        // Update tip text
        UpdateTipText(tipMessage);
    }

    /// <summary>
    ///     Updates the display of the clothing models based on the current clothing type.
    /// </summary>
    /// <param name="currentClothing">The player's current clothing type</param>
    private void DisplayCurrentClothing(ClothingType currentClothing)
    {
        // Show/hide clothing models
        switch (currentClothing)
        {
            case ClothingType.None:
                tShirt.gameObject.SetActive(false);
                winterCoat.gameObject.SetActive(false);
                break;
            case ClothingType.TShirt:
                ShowClothing(tShirt);
                ApplyPulsingEffect(tShirtOutline, currentClothing);
                break;
            case ClothingType.WinterCoat:
                ShowClothing(winterCoat);
                ApplyPulsingEffect(winterCoatOutline, currentClothing);
                break;
        }
    }

    /// <summary>
    ///     Shows the specified clothing model and hides others.
    /// </summary>
    /// <param name="clothingToShow">The transform of the clothing model to show</param>
    private void ShowClothing(Transform clothingToShow)
    {
        tShirt.gameObject.SetActive(clothingToShow == tShirt);
        winterCoat.gameObject.SetActive(clothingToShow == winterCoat);
    }

    /// <summary>
    ///     Applies a pulsing outline effect to clothing when it's inappropriate for the weather.
    /// </summary>
    /// <param name="outlineComponent">The outline component to modify</param>
    /// <param name="clothingType">The current clothing type</param>
    private void ApplyPulsingEffect(Outline outlineComponent, ClothingType clothingType)
    {
        if (outlineComponent == null) return;

        // Check if pulsing should occur
        var shouldPulse = false;
        var currentWeather = weatherManager.CurrentWeather;

        switch (clothingType)
        {
            case ClothingType.TShirt:
                shouldPulse = currentWeather == WeatherType.Snowstorm;
                break;
            case ClothingType.WinterCoat:
                shouldPulse = currentWeather == WeatherType.Heatwave;
                break;
        }

        // Apply pulsing effect
        if (shouldPulse)
        {
            // Calculate pulsing alpha
            var alpha = Mathf.PingPong(Time.time * pulseSpeed, pulseIntensity);

            // Set outline color with pulsing effect
            outlineComponent.effectColor = new Color(
                warningPulseColor.r,
                warningPulseColor.g,
                warningPulseColor.b,
                alpha
            );
        }
        else
        {
            // Reset outline when no pulsing is needed
            outlineComponent.effectColor = new Color(
                outlineComponent.effectColor.r,
                outlineComponent.effectColor.g,
                outlineComponent.effectColor.b,
                0
            );
        }
    }

    /// <summary>
    ///     Updates the clothing tip text with the appropriate message.
    /// </summary>
    /// <param name="message">The message to display, or empty for the default message</param>
    private void UpdateTipText(string message)
    {
        if (clothingTipText == null) return;

        // If no inappropriate clothing and no specific message, show default
        if (string.IsNullOrEmpty(message) && currentClothing != ClothingType.None) message = "Press Tab to remove";

        clothingTipText.text = message;
    }

    /// <summary>
    ///     Handles the Tab key press to remove the player's current clothing.
    /// </summary>
    private void HandleClothingRemoval()
    {
        // Check for Tab key press to remove clothing
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Play sound based on current clothing before removing
            switch (currentClothing)
            {
                case ClothingType.TShirt:
                    playerPickupSounds.PlayPickupSound("TShirt");
                    break;
                case ClothingType.WinterCoat:
                    playerPickupSounds.PlayPickupSound("WinterCoat");
                    break;
            }

            // Reset clothing to None
            playerTemp.ApplyClothingResistance(ClothingType.None);
        }
    }
}