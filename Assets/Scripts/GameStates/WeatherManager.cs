using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// Manages different weather states (Normal, Heatwave, Snowstorm).
/// Controls visual effects, weather cycle, notifications, and audio transitions.
/// </summary>
public class WeatherManager : MonoBehaviour
{
    // =================== Enums & Properties ===================
    
    public WeatherType CurrentWeather { get; private set; }
    
    // =================== External References ===================
    
    [Header("References")]
    public GameFSM gameFSM;
    public SnowEffectController snowEffectController;
    public FogEffectController fogEffectController;
    
    [Header("UI Elements")]
    public TextMeshProUGUI weatherNotificationText;

    [Header("Audio Clips")]
    public AudioClip heatwaveClip;
    public AudioClip snowstormClip;
    public AudioClip backgroundSound;

    [Header("Audio Source")]
    public AudioSource weatherAudioSource;

    // =================== Weather Cycle Settings ===================
    
    [Header("Weather Cycle")]
    [Tooltip("Time in seconds before weather changes automatically.")]
    public float weatherChangeInterval = 30f;
    private Coroutine weatherCoroutine;

    // =================== Notification Fade Settings ===================
    
    [Header("Text Fade Settings")]
    [Tooltip("Time in seconds the notification remains fully visible before fading out.")]
    public float notificationDisplayTime = 2f;
    
    [Tooltip("Duration of the text fade-out.")]
    public float fadeDurationText = 2f;

    // A reference to the currently running notification fade-out (if any).
    private IEnumerator notificationFadeCoroutine = null;

    // =================== Audio Fade Settings ===================
    
    [Header("Audio Fade Settings")]
    [Tooltip("Duration (in seconds) for the audio to fade in.")]
    public float fadeDurationSounds = 1f;

    // A reference to the currently running audio fade-in (if any).
    private IEnumerator soundFadeCoroutine = null;

    // =================== Unity Lifecycle ===================

    private void Start()
    {
        // Attempt to find references if not already set in the Inspector
        if (gameFSM == null)
        {
            gameFSM = FindObjectOfType<GameFSM>();
        }
        if (snowEffectController == null)
        {
            snowEffectController = FindObjectOfType<SnowEffectController>();
        }
        if (fogEffectController == null)
        {
            fogEffectController = FindObjectOfType<FogEffectController>();
        }

        // Initialize the weather to Normal at startup
        SetWeather(WeatherType.Normal);
    }

    // =================== Public Methods ===================

    /// <summary>
    /// Changes weather to the specified type, applies effects, updates text, and plays the proper audio.
    /// </summary>
    public void SetWeather(WeatherType newWeather)
    {
        CurrentWeather = newWeather;
        ApplyWeatherEffects();
        DisplayWeatherNotification(newWeather);
        PlayWeatherSound(newWeather);

        Debug.Log("Weather set to: " + newWeather);
    }

    /// <summary>
    /// Begins automatic weather cycling on a set interval.
    /// </summary>
    public void StartWeatherCycle()
    {
        if (weatherCoroutine == null)
        {
            weatherCoroutine = StartCoroutine(WeatherCycle());
        }
    }

    /// <summary>
    /// Stops automatic weather cycling.
    /// </summary>
    public void StopWeatherCycle()
    {
        if (weatherCoroutine != null)
        {
            StopCoroutine(weatherCoroutine);
            weatherCoroutine = null;
        }
    }

    /// <summary>
    /// Immediately cycles the weather state:
    /// Normal -> (Heatwave or Snowstorm), then returns back to Normal.
    /// </summary>
    public void ChangeWeather()
    {
        if (CurrentWeather == WeatherType.Normal)
        {
            // 50/50 chance for Heatwave vs Snowstorm
            if (Random.value > 0.5f)
            {
                SetWeather(WeatherType.Heatwave);
            }
            else
            {
                SetWeather(WeatherType.Snowstorm);
            }
        }
        else
        {
            // Return to Normal after any extreme weather
            SetWeather(WeatherType.Normal);
        }
    }

    // =================== Private Methods ===================

    /// <summary>
    /// Displays weather notification text and starts a fade-out after a delay.
    /// </summary>
    private void DisplayWeatherNotification(WeatherType weatherType)
    {
        if (weatherNotificationText == null) return;

        // Reset notification text alpha to fully visible
        var color = weatherNotificationText.color;
        weatherNotificationText.color = new Color(color.r, color.g, color.b, 1f);

        // Set message
        switch (weatherType)
        {
            case WeatherType.Normal:
                weatherNotificationText.text = "Mild";
                weatherNotificationText.color = new Color(255, 215, 0);
                break;
            case WeatherType.Heatwave:
                weatherNotificationText.text = "Heatwave";
                weatherNotificationText.color = new Color(255, 69, 0);
                break;
            case WeatherType.Snowstorm:
                weatherNotificationText.text = "Snowstorm";
                weatherNotificationText.color = new Color(0, 255, 255);
                break;
        }

        // Stop any existing notification fade-out, then start a new one
        if (notificationFadeCoroutine != null)
        {
            StopCoroutine(notificationFadeCoroutine);
            notificationFadeCoroutine = null;
        }

        notificationFadeCoroutine = FadeOutNotification(notificationDisplayTime, fadeDurationText);
        StartCoroutine(notificationFadeCoroutine);
    }

    /// <summary>
    /// Plays the appropriate audio clip for the new weather type and smoothly fades it in.
    /// </summary>
    private void PlayWeatherSound(WeatherType newWeather)
    {
        // Stop any existing fade-in
        if (soundFadeCoroutine != null)
        {
            StopCoroutine(soundFadeCoroutine);
            soundFadeCoroutine = null;
        }

        // Stop any currently playing sound
        if (weatherAudioSource.isPlaying)
        {
            weatherAudioSource.Stop();
        }

        // Assign the correct clip based on weather
        switch (newWeather)
        {
            case WeatherType.Heatwave:
                weatherAudioSource.clip = heatwaveClip;
                break;
            case WeatherType.Snowstorm:
                weatherAudioSource.clip = snowstormClip;
                break;
            case WeatherType.Normal:
                weatherAudioSource.clip = backgroundSound;
                break;
        }

        // Start at volume=0, play the clip, then fade in
        weatherAudioSource.volume = 0f;
        weatherAudioSource.Play();

        soundFadeCoroutine = FadeInSound(fadeDurationSounds);
        StartCoroutine(soundFadeCoroutine);
    }

    /// <summary>
    /// Toggles visual effects based on the current weather type.
    /// </summary>
    private void ApplyWeatherEffects()
    {
        // Turn off both effects by default
        if (snowEffectController) snowEffectController.SetVisible(false);
        if (fogEffectController)  fogEffectController.SetVisible(false);

        // Enable the one relevant effect
        switch (CurrentWeather)
        {
            case WeatherType.Heatwave:
                fogEffectController?.SetVisible(true);
                break;
            case WeatherType.Snowstorm:
                snowEffectController?.SetVisible(true);
                break;
            case WeatherType.Normal:
                // no special effect
                break;
        }
    }

    /// <summary>
    /// Repeatedly waits for weatherChangeInterval, then auto-changes weather.
    /// </summary>
    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(weatherChangeInterval);
            ChangeWeather();
        }
    }

    // =================== Coroutines ===================

    /// <summary>
    /// Fades out the notification text (alpha from 1 to 0) after a short delay.
    /// </summary>
    private IEnumerator FadeOutNotification(float delay, float duration)
    {
        // Wait before starting fade
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        Color originalColor = weatherNotificationText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            weatherNotificationText.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha
            );
            yield return null;
        }

        // Optionally clear the text
        weatherNotificationText.text = "";
    }

    /// <summary>
    /// Gradually fades an AudioSource from 0 up to full volume over 'duration' seconds.
    /// </summary>
    private IEnumerator FadeInSound(float duration)
    {
        float elapsed = 0f;
        weatherAudioSource.volume = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newVolume = Mathf.Clamp01(elapsed / duration);
            weatherAudioSource.volume = newVolume;
            yield return null;
        }

        // Ensure we end fully at volume = 1
        weatherAudioSource.volume = 1f;
    }
}