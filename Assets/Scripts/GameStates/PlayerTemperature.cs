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

    // Reference to WeatherManager
    private WeatherManager weatherManager;

    private GameFSM gameFSM;
    private GameOverManager gameOverManager;
    
    void Start()
    {
        weatherManager = FindObjectOfType<WeatherManager>();
        gameFSM = FindObjectOfType<GameFSM>();
        gameOverManager = FindObjectOfType<GameOverManager>();
    }

    void Update()
    {
        // Adjust temperature based on current weather
        switch (weatherManager.CurrentWeather)
        {
            case WeatherType.Heatwave:
                bodyTemperature += heatwaveRate * Time.deltaTime;
                break;
            case WeatherType.Normal:
                // No change
                break;
            case WeatherType.Snowstorm:
                bodyTemperature += snowstormRate * Time.deltaTime;
                break;
        }

        // Clamp temperature
        bodyTemperature = Mathf.Clamp(bodyTemperature, minTemperature, maxTemperature);

        // Check for game over conditions
        if (!gameOverTriggered && (bodyTemperature <= minTemperature || bodyTemperature >= maxTemperature))
        {
            gameOverTriggered = true;
            Debug.Log("Player has died due to extreme temperature!");
            // Transition the FSM to GameOverState
            if (gameFSM != null)
            {
                gameFSM.TransitionToState(gameFSM.GameOverState);
            }
            // Show the GameOver panel
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver();
            }
        }

        // Update UI (e.g., temperature bar) here
    }

    // Method to adjust temperature based on items (clothing, drinks)
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

    // Example: Apply clothing resistance
    public void ApplyClothingResistance(ClothingType clothing)
    {
        switch (clothing)
        {
            case ClothingType.TShirt:
                ModifyTemperature(1f); // Mild effect
                break;
            case ClothingType.WinterCoat:
                ModifyTemperature(4f); // Strong effect
                break;
            // Add more clothing types as needed
        }
    }

    // Example: Apply drink effects
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
            // Add more drink types as needed
        }
    }

    // Emergency Kit
    public void UseEmergencyKit()
    {
        ResetTemperature();
    }
}
