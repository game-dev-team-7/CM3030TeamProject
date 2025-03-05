using UnityEngine;

/// <summary>
/// Manages player's body temperature in response to weather conditions.
/// Implements strategic temperature changes with non-linear progression and 
/// clothing-based modifiers to create dynamic gameplay challenge.
/// </summary>
public class PlayerTemperature : MonoBehaviour
{
    public float bodyTemperature = 0f; // Starts at 0
    public float minTemperature = -100f;
    public float maxTemperature = 100f;

    //Temperature change rates per second depending on weather event
    [Header("Temperature Change Rates")]
    public float baseHeatwaveRate = 2f; //Both rates give an increased urgency feeling
    public float baseSnowstormRate = -2f;
    
    //Additional rate multipliers for inappropriate clothing
    [Header("Clothing Penalty Multipliers")]
    public float inappropriateClothingMultiplier = 1.5f; // Higher penalties for wrong clothing
    
    //Internal variables
    private bool gameOverTriggered = false;
    private float lastTemperatureUpdate = 0f;
    private float temperatureUpdateInterval = 0.1f; //Updates more frequently

    //Reference to WeatherManager and other systems
    private WeatherManager weatherManager;
    private GameFSM gameFSM;
    [SerializeField] private InGameMenuManager gameOverManager;
    
    //Currently worn clothing
    public ClothingType currentClothing = ClothingType.None;

    //Event for other systems to listen to temperature changes
    public delegate void TemperatureChangeHandler();
    public event TemperatureChangeHandler OnTemperatureChanged;

    void Start()
    {
        weatherManager = FindObjectOfType<WeatherManager>();
        gameFSM = FindObjectOfType<GameFSM>();
        
        // Initialize UI
        UpdateTemperatureUI();
    }

    void Update()
    {
        // Only update temperature at specific intervals (performance optimization)
        if (Time.time >= lastTemperatureUpdate + temperatureUpdateInterval)
        {
            lastTemperatureUpdate = Time.time;
            UpdatePlayerTemperature();
        }
    }
    
    /// <summary>
    /// Updates player temperature based on weather conditions and clothing.
    /// Implements non-linear progression where extreme temperatures change faster.
    /// </summary>
    void UpdatePlayerTemperature()
    {
        // Get current weather from manager
        WeatherType currentWeather = weatherManager.CurrentWeather;
        
        // Base temperature change based on weather
        float weatherEffect = 0f;
        
        if (currentWeather == WeatherType.Heatwave)
            weatherEffect = baseHeatwaveRate;
        else if (currentWeather == WeatherType.Snowstorm)
            weatherEffect = baseSnowstormRate;
        
        // Apply clothing effects (more severe penalties for inappropriate clothing)
        float multiplier = GetEnhancedClothingMultiplier(currentWeather, currentClothing);
        
        // Calculate final temperature change
        float temperatureChange = weatherEffect * multiplier * temperatureUpdateInterval;
        
        // Apply temperature change if not in normal weather
        if (currentWeather != WeatherType.Normal)
        {
            bodyTemperature += temperatureChange;
        }
        else
        {
            // In normal weather, slowly return to neutral (0)
            if (Mathf.Abs(bodyTemperature) > 0.1f)
            {
                // Move towards 0 at a moderate rate
                float recoveryRate = 2f * temperatureUpdateInterval;
                bodyTemperature = Mathf.MoveTowards(bodyTemperature, 0f, recoveryRate);
            }
        }

        // Clamp temperature
        bodyTemperature = Mathf.Clamp(bodyTemperature, minTemperature, maxTemperature);
        
        // Update UI display
        UpdateTemperatureUI();

        // Check for game over conditions
        if (!gameOverTriggered && (bodyTemperature <= minTemperature || bodyTemperature >= maxTemperature))
        {
            gameOverTriggered = true;
            string deathCause = bodyTemperature <= minTemperature ? "hypothermia" : "hyperthermia";
            Debug.Log($"Player has died due to extreme {deathCause}!");
            
            if (gameFSM != null)
            {
                gameFSM.TransitionToState(gameFSM.GameOverState);
            }
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver();
                
                // Set more specific game over message based on death cause
                if (gameOverManager.gameOverText != null)
                {
                    if (deathCause == "hypothermia")
                    {
                        gameOverManager.gameOverText.text = "You froze to death!";
                    }
                    else
                    {
                        gameOverManager.gameOverText.text = "You died from heatstroke!";
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Calculates clothing multiplier with enhanced penalties for inappropriate choices.
    /// Implements non-linear effects where extreme temperatures amplify clothing penalties.
    /// </summary>
    private float GetEnhancedClothingMultiplier(WeatherType weather, ClothingType clothing)
    {
        // Default multiplier is 1 (no change)
        float multiplier = 1f;
        
        switch(clothing)
        {
            case ClothingType.TShirt:
                if (weather == WeatherType.Heatwave)
                    multiplier = 0.6f;  // Some benefit in heat
                else if (weather == WeatherType.Snowstorm)
                    multiplier = inappropriateClothingMultiplier;  // Severe penalty in cold
                break;
                
            case ClothingType.WinterCoat:
                if (weather == WeatherType.Snowstorm)
                    multiplier = 0.6f;  // Some benefit in cold
                else if (weather == WeatherType.Heatwave)
                    multiplier = inappropriateClothingMultiplier;  // Severe penalty in heat
                break;
                
            // In case of "None" or any other clothing type, slightly worse than proper clothing
            default:
                multiplier = 1.2f;
                break;
        }
        
        // Apply additional multiplier based on current temperature
        // This creates a non-linear effect - the more extreme your temperature,
        // the faster it gets worse (simulating body struggling to regulate)
        
        // Get normalized temperature (-1 to 1 range)
        float normalizedTemp = bodyTemperature / maxTemperature;
        
        // If temperature is already extreme in the same direction as the weather effect
        if ((normalizedTemp > 0.6f && weather == WeatherType.Heatwave) || 
            (normalizedTemp < -0.6f && weather == WeatherType.Snowstorm))
        {
            // Add up to 20% extra penalty based on how extreme current temperature is
            float extremityMultiplier = 1f + (Mathf.Abs(normalizedTemp) * 0.2f);
            multiplier *= extremityMultiplier;
        }
        
        return multiplier;
    }
    
    /// <summary>
    /// Notifies other systems about temperature changes.
    /// </summary>
    private void UpdateTemperatureUI()
    {
        // We're not updating UI directly as TemperatureBarManager handles this
        // This method remains for API compatibility but with minimal implementation
        
        // Notify other systems that temperature has changed
        if (OnTemperatureChanged != null)
        {
            OnTemperatureChanged.Invoke();
        }
    }

    /// <summary>
    /// Instantly modifies player temperature (for consumable items).
    /// </summary>
    public void ModifyTemperature(float amount)
    {
        bodyTemperature += amount;
        bodyTemperature = Mathf.Clamp(bodyTemperature, minTemperature, maxTemperature);
        UpdateTemperatureUI();
    }

    /// <summary>
    /// Resets temperature to neutral (0).
    /// </summary>
    public void ResetTemperature()
    {
        bodyTemperature = 0f;
        UpdateTemperatureUI();
    }

    /// <summary>
    /// Sets the current clothing type and notifies listeners.
    /// </summary>
    public void ApplyClothingResistance(ClothingType clothing)
    {
        currentClothing = clothing;
        
        // Notify listeners about the change
        if (OnTemperatureChanged != null)
        {
            OnTemperatureChanged.Invoke();
        }
    }

    /// <summary>
    /// Applies immediate cooling or warming effect from drinks.
    /// Enhanced values for more significant gameplay impact.
    /// </summary>
    public void ApplyDrinkEffect(DrinkType drink)
    {
        switch (drink)
        {
            case DrinkType.Lemonade:
                ModifyTemperature(-30f); // Significant immediate cooling
                break;
            case DrinkType.HotChocolate:
                ModifyTemperature(30f); // Significant immediate warming
                break;
        }
    }

    /// <summary>
    /// Emergency kit: resets temperature and provides temporary resistance.
    /// </summary>
    public void UseEmergencyKit()
    {
        ResetTemperature();
        
        // Provide temporary resistance to extreme temperatures
        StartCoroutine(TemporaryTemperatureResistance(15f)); // 15 seconds of resistance
    }
    
    /// <summary>
    /// Provides temporary resistance to temperature changes after emergency kit use.
    /// </summary>
    private System.Collections.IEnumerator TemporaryTemperatureResistance(float duration)
    {
        // Store original rates
        float originalHeatwaveRate = baseHeatwaveRate;
        float originalSnowstormRate = baseSnowstormRate;
        
        // Reduce rates by 75%
        baseHeatwaveRate *= 0.25f;
        baseSnowstormRate *= 0.25f;
        
        // Wait for duration
        yield return new WaitForSeconds(duration);
        
        // Restore original rates
        baseHeatwaveRate = originalHeatwaveRate;
        baseSnowstormRate = originalSnowstormRate;
    }
}