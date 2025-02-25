using UnityEngine;
using System.Collections;
using TMPro;

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

    public SnowEffectController snowEffectController;
    public FogEffectController fogEffectController;
    
    public TextMeshProUGUI weatherNotificationText;
    
    public float notificationDisplayTime = 2f; // Seconds before starting to fade out
    public float fadeDuration = 2f;            // Duration of the fade out effect

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

        if (weatherNotificationText != null)
        {
            // Reset text alpha to 1 (fully visible)
            Color color = weatherNotificationText.color;
            weatherNotificationText.color = new Color(color.r, color.g, color.b, 1f);

            // Update the text message based on the weather type
            switch (newWeather)
            {
                case WeatherType.Normal:
                    weatherNotificationText.text = "Weather is mild";
                    break;
                case WeatherType.Heatwave:
                    weatherNotificationText.text = "Heat wave started";
                    break;
                case WeatherType.Snowstorm:
                    weatherNotificationText.text = "Snowstorm started";
                    break;
            }
        
            // Optional: Stop any running fade out coroutines if you want to reset the timer.
            StopAllCoroutines();
            // Start the fade out after the specified display time.
            StartCoroutine(FadeOutNotification(notificationDisplayTime, fadeDuration));
        }
    
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
    
    private IEnumerator FadeOutNotification(float delay, float duration)
    {
        // Wait for a short delay before starting the fade-out.
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        Color originalColor = weatherNotificationText.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Interpolate alpha from 1 (fully opaque) to 0 (fully transparent)
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            weatherNotificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    
        // Optionally clear the text after fade out
        weatherNotificationText.text = "";
    }
}