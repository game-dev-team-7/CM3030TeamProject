// WeatherManager.cs
using UnityEngine;
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    public WeatherType CurrentWeather { get; private set; }

    // Weather Change Interval (in seconds)
    public float weatherChangeInterval = 30f;

    // Coroutine reference
    private Coroutine weatherCoroutine;

    // Reference to GameFSM
    public GameFSM gameFSM;

    // Reference to WeatherTintController
    public WeatherTintController tintController;

    void Start()
    {
        // Ensure WeatherManager is linked to GameFSM
        if (gameFSM == null)
        {
            gameFSM = FindObjectOfType<GameFSM>();
        }

        // Ensure WeatherTintController is linked
        if (tintController == null)
        {
            tintController = FindObjectOfType<WeatherTintController>();
        }
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
        // Simple example: Cycle through Mild → Normal → Heatwave → Snowstorm → Mild
        switch (CurrentWeather)
        {
            case WeatherType.Mild:
                SetWeather(WeatherType.Normal);
                break;
            case WeatherType.Normal:
                // Randomly choose between Heatwave and Snowstorm
                if (Random.value > 0.5f)
                    SetWeather(WeatherType.Heatwave);
                else
                    SetWeather(WeatherType.Snowstorm);
                break;
            case WeatherType.Heatwave:
                SetWeather(WeatherType.Normal);
                break;
            case WeatherType.Snowstorm:
                SetWeather(WeatherType.Normal);
                break;
        }
    }

    private void ApplyWeatherEffects()
    {
        // Implement visual and gameplay effects based on CurrentWeather
        // Instead of changing skybox, change screen tint
        switch (CurrentWeather)
        {
            case WeatherType.Mild:
                tintController.ChangeTint(new Color(0, 1, 1, 0.2f)); // Light Cyan
                break;
            case WeatherType.Normal:
                tintController.ChangeTint(new Color(0, 0, 1, 0.2f)); // Light Blue
                break;
            case WeatherType.Heatwave:
                tintController.ChangeTint(new Color(1, 0, 0, 0.3f)); // Light Red
                break;
            case WeatherType.Snowstorm:
                tintController.ChangeTint(new Color(1, 1, 1, 0.3f)); // Light White
                break;
        }

        // Additional gameplay effects can be handled here
        // e.g., Adjust temperature rates, spawn weather-related particles, etc.
    }
}
