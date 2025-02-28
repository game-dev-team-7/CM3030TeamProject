using UnityEngine;

public class PlayerTemperature : MonoBehaviour
{
    public float bodyTemperature = 0f; // Starts at 0
    public float minTemperature = -100f;
    public float maxTemperature = 100f;

    // Temperature change rates per second (adjustable in Inspector)
    [Header("Temperature Change Rates")]
    public float heatwaveRate = 3f; // Increased from 1f
    public float snowstormRate = -3f; // Increased from -1f
    private bool gameOverTriggered = false; // Ensure we only trigger game over once

    // Reference to WeatherManager and other systems
    private WeatherManager weatherManager;
    private GameFSM gameFSM;
    [SerializeField] private InGameMenuManager gameOverManager;
    
    // NEW: Currently worn clothing, set externally via ApplyClothingResistance
    public ClothingType currentClothing = ClothingType.None; // Assuming a "None" option exists

    void Start()
    {
        weatherManager = FindObjectOfType<WeatherManager>();
        gameFSM = FindObjectOfType<GameFSM>();
    }

    void Update()
    {
        // Adjust temperature continuously based on current weather and clothing
        float weatherEffect = 0f;
        WeatherType currentWeather = weatherManager.CurrentWeather;
        
        if (currentWeather == WeatherType.Heatwave)
            weatherEffect = heatwaveRate;
        else if (currentWeather == WeatherType.Snowstorm)
            weatherEffect = snowstormRate;
        
        // Only modify if weather is not normal (normal implies no weather-based change)
        if (currentWeather != WeatherType.Normal)
        {
            float multiplier = GetClothingMultiplier(currentWeather, currentClothing);
            bodyTemperature += weatherEffect * multiplier * Time.deltaTime;
        }

        // Clamp temperature
        bodyTemperature = Mathf.Clamp(bodyTemperature, minTemperature, maxTemperature);

        // Check for game over conditions
        if (!gameOverTriggered && (bodyTemperature <= minTemperature || bodyTemperature >= maxTemperature))
        {
            gameOverTriggered = true;
            Debug.Log("Player has died due to extreme temperature!");
            if (gameFSM != null)
            {
                gameFSM.TransitionToState(gameFSM.GameOverState);
            }
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver();
            }
        }

        // Update UI (e.g., temperature bar) here
    }

    // Helper method to compute a multiplier based on weather and clothing worn
    private float GetClothingMultiplier(WeatherType weather, ClothingType clothing)
    {
        // Default multiplier is 1 (no change)
        float multiplier = 1f;
        switch(clothing)
        {
            case ClothingType.TShirt:
                if (weather == WeatherType.Heatwave)
                    multiplier = 0.5f;  // slows down the temperature increase
                else if (weather == WeatherType.Snowstorm)
                    multiplier = 1.5f;  // accelerates the temperature decrease
                break;
            case ClothingType.WinterCoat:
                if (weather == WeatherType.Snowstorm)
                    multiplier = 0.5f;  // slows down the temperature decrease
                else if (weather == WeatherType.Heatwave)
                    multiplier = 1.5f;  // accelerates the temperature increase
                break;
            // In case of "None" or any other clothing type, use default multiplier 1
            default:
                multiplier = 1f;
                break;
        }
        return multiplier;
    }

    // Method to adjust temperature based on one-time effects (e.g., drinks)
    public void ModifyTemperature(float amount)
    {
        bodyTemperature += amount;
        bodyTemperature = Mathf.Clamp(bodyTemperature, minTemperature, maxTemperature);
    }

    // Optional: Reset temperature
    public void ResetTemperature()
    {
        bodyTemperature = 0f;
    }

    // Modified: Set the current clothing, which now affects the weather-driven rate continuously.
    public void ApplyClothingResistance(ClothingType clothing)
    {
        currentClothing = clothing;
    }

    // Apply drink effects (one-time boosts remain the same)
    public void ApplyDrinkEffect(DrinkType drink)
    {
        switch (drink)
        {
            case DrinkType.Lemonade:
                ModifyTemperature(-20f); // Decrease temperature
                break;
            case DrinkType.HotChocolate:
                ModifyTemperature(20f); // Increase temperature
                break;
        }
    }

    // Emergency Kit: one-time reset effect remains unchanged
    public void UseEmergencyKit()
    {
        ResetTemperature();
    }
}