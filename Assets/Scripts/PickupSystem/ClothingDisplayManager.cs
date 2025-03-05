using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClothingDisplayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerTemperature playerTemp;
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private PlayerPickupSounds playerPickupSounds;
    [SerializeField] private Transform tShirt;
    [SerializeField] private Transform winterCoat;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI clothingTipText;
    
    [Header("Pulsing Effect Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseIntensity = 0.5f;
    [SerializeField] private Color warningPulseColor = Color.red;

    [Header("Outline Components")]
    [SerializeField] private Outline tShirtOutline;
    [SerializeField] private Outline winterCoatOutline;

    private ClothingType currentClothing = ClothingType.None;
    private bool isInappropriateClothing = false;

    void Start()
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
        if (clothingTipText == null)
        {
            Debug.LogError("[ClothingDisplayManager] Clothing tip text is not assigned!");
        }
    }

    void Update()
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

    void CheckClothingAppropriate()
    {
        WeatherType currentWeather = weatherManager.CurrentWeather;
        isInappropriateClothing = false;
        string tipMessage = "";

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
                    tipMessage = "Take off the winter coat!";
                }
                break;
        }

        // Update tip text
        UpdateTipText(tipMessage);
    }

    void DisplayCurrentClothing(ClothingType currentClothing)
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

    void ShowClothing(Transform clothingToShow)
    {
        tShirt.gameObject.SetActive(clothingToShow == tShirt);
        winterCoat.gameObject.SetActive(clothingToShow == winterCoat);
    }

    void ApplyPulsingEffect(Outline outlineComponent, ClothingType clothingType)
    {
        if (outlineComponent == null) return;

        // Check if pulsing should occur
        bool shouldPulse = false;
        WeatherType currentWeather = weatherManager.CurrentWeather;

        switch (clothingType)
        {
            case ClothingType.TShirt:
                shouldPulse = (currentWeather == WeatherType.Snowstorm);
                break;
            case ClothingType.WinterCoat:
                shouldPulse = (currentWeather == WeatherType.Heatwave);
                break;
        }

        // Apply pulsing effect
        if (shouldPulse)
        {
            // Calculate pulsing alpha
            float alpha = Mathf.PingPong(Time.time * pulseSpeed, pulseIntensity);
            
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

    void UpdateTipText(string message)
    {
        if (clothingTipText == null) return;

        // If no inappropriate clothing and no specific message, show default
        if (string.IsNullOrEmpty(message) && currentClothing != ClothingType.None)
        {
            message = "Press Tab to remove";
        }

        clothingTipText.text = message;
    }

    void HandleClothingRemoval()
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