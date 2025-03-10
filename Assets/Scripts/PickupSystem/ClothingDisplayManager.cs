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

    // Cache the current weather to avoid repeated lookups
    private WeatherType currentWeather;

    /// <summary>
    ///     Initializes the component and validates required references.
    /// </summary>
    private void Start()
    {
        ValidateReferences();
    }

    /// <summary>
    ///     Updates the clothing display each frame based on the player's current clothing.
    /// </summary>
    private void Update()
    {
        // Skip processing if essential components are missing
        if (playerTemp == null || weatherManager == null) return;

        // Update current clothing and weather (cached to avoid repeated lookups)
        currentClothing = playerTemp.currentClothing;
        currentWeather = weatherManager.CurrentWeather;

        // Check for inappropriate clothing and update display
        CheckClothingAppropriate();
        DisplayCurrentClothing(currentClothing);

        // Handle Tab key for removing clothing
        HandleClothingRemoval();
    }

    /// <summary>
    ///     Validates that all required references are set.
    /// </summary>
    private void ValidateReferences()
    {
        if (playerTemp == null) Debug.LogError("[ClothingDisplayManager] PlayerTemperature script not found!");

        if (weatherManager == null) Debug.LogError("[ClothingDisplayManager] WeatherManager script not found!");

        if (playerPickupSounds == null) Debug.LogError("[ClothingDisplayManager] PlayerPickupSounds script not found!");

        if (clothingTipText == null) Debug.LogError("[ClothingDisplayManager] Clothing tip text is not assigned!");
    }

    /// <summary>
    ///     Checks if the current clothing is appropriate for the weather conditions and updates tip text.
    /// </summary>
    private void CheckClothingAppropriate()
    {
        var tipMessage = "";

        // Determine appropriate tip message based on clothing and weather combination
        switch (currentClothing)
        {
            case ClothingType.TShirt:
                if (currentWeather == WeatherType.Snowstorm)
                    tipMessage = "Press Tab to remove!";
                else if (currentWeather == WeatherType.Normal) tipMessage = "Press Tab to remove";
                break;

            case ClothingType.WinterCoat:
                if (currentWeather == WeatherType.Heatwave)
                    tipMessage = "Press Tab to remove!";
                else if (currentWeather == WeatherType.Normal) tipMessage = "Press Tab to remove";
                break;

            case ClothingType.None:
                if (currentWeather == WeatherType.Snowstorm)
                    tipMessage = "Put on a Winter Coat!";
                else if (currentWeather == WeatherType.Heatwave) tipMessage = "Put on a T-Shirt!";
                break;
        }

        // Update tip text
        UpdateTipText(tipMessage);
    }

    /// <summary>
    ///     Updates the display of the clothing models based on the current clothing type.
    /// </summary>
    /// <param name="clothingType">The player's current clothing type</param>
    private void DisplayCurrentClothing(ClothingType clothingType)
    {
        // Hide all clothing models by default
        var showTShirt = false;
        var showWinterCoat = false;

        // Determine which clothing to show based on type
        switch (clothingType)
        {
            case ClothingType.TShirt:
                showTShirt = true;
                if (tShirtOutline != null) ApplyPulsingEffect(tShirtOutline, clothingType);
                break;

            case ClothingType.WinterCoat:
                showWinterCoat = true;
                if (winterCoatOutline != null) ApplyPulsingEffect(winterCoatOutline, clothingType);
                break;
        }

        // Update visibility state (only do SetActive calls when necessary)
        if (tShirt.gameObject.activeSelf != showTShirt) tShirt.gameObject.SetActive(showTShirt);

        if (winterCoat.gameObject.activeSelf != showWinterCoat) winterCoat.gameObject.SetActive(showWinterCoat);
    }

    /// <summary>
    ///     Applies a pulsing outline effect to clothing when it's inappropriate for the weather.
    /// </summary>
    /// <param name="outlineComponent">The outline component to modify</param>
    /// <param name="clothingType">The current clothing type</param>
    private void ApplyPulsingEffect(Outline outlineComponent, ClothingType clothingType)
    {
        // Determine if this clothing type should pulse based on current weather
        var shouldPulse = (clothingType == ClothingType.TShirt && currentWeather == WeatherType.Snowstorm) ||
                          (clothingType == ClothingType.WinterCoat && currentWeather == WeatherType.Heatwave);

        // Apply appropriate effect to the outline
        var outlineColor = outlineComponent.effectColor;

        if (shouldPulse)
        {
            // Calculate pulsing alpha using PingPong
            var alpha = Mathf.PingPong(Time.time * pulseSpeed, pulseIntensity);

            // Apply warning color with pulsing alpha
            outlineColor = new Color(
                warningPulseColor.r,
                warningPulseColor.g,
                warningPulseColor.b,
                alpha
            );
        }
        else
        {
            // Reset outline to be invisible when no warning needed
            outlineColor.a = 0;
        }

        // Only update the color if it changed to avoid unnecessary operations
        if (outlineComponent.effectColor != outlineColor) outlineComponent.effectColor = outlineColor;
    }

    /// <summary>
    ///     Updates the clothing tip text with the appropriate message.
    /// </summary>
    /// <param name="message">The message to display, or empty for the default message</param>
    private void UpdateTipText(string message)
    {
        if (clothingTipText == null) return;

        // If no specific message and player has clothing, show default removal message
        if (string.IsNullOrEmpty(message) && currentClothing != ClothingType.None) message = "Press Tab to remove";

        // Only update the text component if the message has changed
        if (clothingTipText.text != message) clothingTipText.text = message;
    }

    /// <summary>
    ///     Handles the Tab key press to remove the player's current clothing.
    /// </summary>
    private void HandleClothingRemoval()
    {
        // Check for Tab key press to remove clothing
        if (Input.GetKeyDown(KeyCode.Tab) && currentClothing != ClothingType.None && playerPickupSounds != null)
        {
            // Play sound based on current clothing
            var soundName = currentClothing == ClothingType.TShirt ? "TShirt" : "WinterCoat";
            playerPickupSounds.PlayPickupSound(soundName);

            // Reset clothing to None
            playerTemp.ApplyClothingResistance(ClothingType.None);
        }
    }
}