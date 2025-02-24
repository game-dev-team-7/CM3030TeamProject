using UnityEngine;
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    // Updated WeatherType enum should only include Normal, Heatwave, and Snowstorm.
    public WeatherType CurrentWeather { get; private set; }

    // Weather Change Interval (in seconds)
    public float weatherChangeInterval = 30f;

    // Coroutine reference for weather cycle
    private Coroutine weatherCoroutine;

    // Reference to GameFSM (if needed)
    public GameFSM gameFSM;

    // New effect controller references
    public SnowEffectController snowEffectController;
    public FogEffectController fogEffectController;

    void Start()
    {
        // Link to GameFSM if not already set.
        if (gameFSM == null)
        {
            gameFSM = FindObjectOfType<GameFSM>();
        }

        // Automatically find the effect controllers if not linked manually.
        if (snowEffectController == null)
        {
            snowEffectController = FindObjectOfType<SnowEffectController>();
        }
        if (fogEffectController == null)
        {
            fogEffectController = FindObjectOfType<FogEffectController>();
        }
        
        // Start with Normal weather (no extra effects)
        SetWeather(WeatherType.Normal);
    }

    public void SetWeather(WeatherType newWeather)
    {
        CurrentWeather = newWeather;
        ApplyWeatherEffects();
        Debug.Log("Weather set to: " + newWeather);
    }

    public void StartWeatherCycle()
    {
        if (weatherCoroutine == null)
        {
            weatherCoroutine = StartCoroutine(WeatherCycle());
        }
    }

    public void StopWeatherCycle()
    {
        if (weatherCoroutine != null)
        {
            StopCoroutine(weatherCoroutine);
            weatherCoroutine = null;
        }
    }

    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(weatherChangeInterval);
            ChangeWeather();
        }
    }

    public void ChangeWeather()
    {
        // Cycle between Normal and either Heatwave or Snowstorm.
        if (CurrentWeather == WeatherType.Normal)
        {
            // Randomly choose between Heatwave and Snowstorm.
            if (Random.value > 0.5f)
                SetWeather(WeatherType.Heatwave);
            else
                SetWeather(WeatherType.Snowstorm);
        }
        else
        {
            // Return to Normal weather after a heatwave or snowstorm.
            SetWeather(WeatherType.Normal);
        }
    }

    private void ApplyWeatherEffects()
    {
        // Turn off both effects by default.
        snowEffectController.SetVisible(false);
        fogEffectController.SetVisible(false);

        // Enable the appropriate effect based on current weather.
        switch (CurrentWeather)
        {
            case WeatherType.Normal:
                // No weather effect.
                break;
            case WeatherType.Heatwave:
                // Show fog effect during a heatwave.
                fogEffectController.SetVisible(true);
                break;
            case WeatherType.Snowstorm:
                // Show snow effect during a snowstorm.
                snowEffectController.SetVisible(true);
                break;
        }
    }
}