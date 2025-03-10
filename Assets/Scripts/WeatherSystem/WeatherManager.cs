using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
///     Manages different weather states (Normal, Heatwave, Snowstorm).
///     Implements game theory principles to create dynamic weather challenges that
///     respond strategically to player adaptation.
/// </summary>
public class WeatherManager : MonoBehaviour
{
    // =================== External References ===================

    [Header("References")] public GameFSM gameFSM;

    public GlobalSnowController globalSnowController;
    public PlayerSnowController playerSnowController;
    public FogEffectController fogEffectController;

    [Header("UI Elements")] public TextMeshProUGUI weatherNotificationText;

    public GameObject weatherNotificationBackground;

    [Header("Audio Clips")] public AudioClip heatwaveClip;

    public AudioClip snowstormClip;
    public AudioClip backgroundSound;

    [Header("Audio Source")] public AudioSource weatherAudioSource;

    // =================== Weather Cycle Settings ===================

    [Header("Weather Cycle")] [Tooltip("Base time in seconds before weather changes automatically.")]
    public float baseWeatherChangeInterval = 30f;

    [Tooltip("Minimum time in seconds for weather changes.")]
    public float minWeatherChangeInterval = 15f;

    [Tooltip("Maximum time in seconds for weather changes.")]
    public float maxWeatherChangeInterval = 45f;

    [Tooltip("Chance of giving the player a relaxation period.")] [Range(0f, 1f)]
    public float relaxationChance = 0.2f;

    [Tooltip("Duration multiplier for relaxation periods.")]
    public float relaxationDurationMultiplier = 1.5f;

    [Tooltip("Maximum times the same weather type can occur consecutively.")]
    public int maxSameWeatherCount = 2;

    // =================== Player Monitoring ===================
    [Header("Player Monitoring")] public PlayerTemperature playerTemperature;
    

    // =================== Audio Fade Settings ===================

    [Header("Audio Fade Settings")] [Tooltip("Duration (in seconds) for the audio to fade in.")]
    public float fadeDurationSounds = 1f;

    private readonly float adaptationThreshold = 0.7f; // Threshold to consider player well-adapted
    private int consecutiveAdverseWeatherCount;
    private ClothingType lastPlayerClothing = ClothingType.None;
    private WeatherType lastWeatherType;
    private readonly int maxConsecutiveAdverseWeather = 3; // Max number of consecutive adverse weather events
    private float nextWeatherChangeTime;

    // A reference to the currently running notification fade-out (if any).
    private float playerAdaptationScore;
    private int sameWeatherTypeCount;

    // A reference to the currently running audio fade-in (if any).
    private IEnumerator soundFadeCoroutine;

    private Coroutine weatherCoroutine;
    // =================== Enums & Properties ===================

    public WeatherType CurrentWeather { get; private set; }

    // =================== Unity Lifecycle ===================

    private void Start()
    {
        // Attempt to find references if not already set in the Inspector
        if (gameFSM == null) gameFSM = FindObjectOfType<GameFSM>();
        if (globalSnowController == null) globalSnowController = FindObjectOfType<GlobalSnowController>();
        if (playerSnowController == null) playerSnowController = FindObjectOfType<PlayerSnowController>();
        if (fogEffectController == null) fogEffectController = FindObjectOfType<FogEffectController>();
        if (playerTemperature == null) playerTemperature = FindObjectOfType<PlayerTemperature>();

        // Initialize the weather to Normal at startup
        SetWeather(WeatherType.Normal);

        // Store initial weather type
        lastWeatherType = WeatherType.Normal;
        sameWeatherTypeCount = 1;

        // Initialize next weather change time
        nextWeatherChangeTime = Time.time + baseWeatherChangeInterval;
    }

    private void Update()
    {
        // Monitor player adaptation to weather
        if (playerTemperature != null) MonitorPlayerAdaptation();
    }

    // =================== Public Methods ===================

    /// <summary>
    ///     Changes weather to the specified type, applies effects, updates text, and plays the proper audio.
    /// </summary>
    public void SetWeather(WeatherType newWeather)
    {
        // Check if we're setting the same weather type
        if (newWeather == CurrentWeather)
        {
            sameWeatherTypeCount++;
        }
        else
        {
            sameWeatherTypeCount = 1;
            lastWeatherType = CurrentWeather;
        }

        CurrentWeather = newWeather;
        ApplyWeatherEffects();
        DisplayWeatherNotification(newWeather);
        PlayWeatherSound(newWeather);

        // Track consecutive adverse weather events
        if (newWeather != WeatherType.Normal)
            consecutiveAdverseWeatherCount++;
        else
            consecutiveAdverseWeatherCount = 0;

        Debug.Log("Weather set to: " + newWeather + " (Same type count: " + sameWeatherTypeCount + ")");
    }

    /// <summary>
    ///     Begins adaptive weather cycling that responds to player behavior.
    /// </summary>
    public void StartWeatherCycle()
    {
        if (weatherCoroutine == null) weatherCoroutine = StartCoroutine(AdaptiveWeatherCycle());
    }

    /// <summary>
    ///     Stops automatic weather cycling.
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
    ///     Intelligently selects the next weather based on player adaptation and game theory principles.
    /// </summary>
    public void StrategicWeatherChange()
    {
        // Force a change if we've had the same weather type for too long
        if (sameWeatherTypeCount >= maxSameWeatherCount)
        {
            Debug.Log("Forcing weather change after " + sameWeatherTypeCount + " cycles of the same weather");

            // If we're in normal weather too long, switch to an adverse weather
            if (CurrentWeather == WeatherType.Normal)
            {
                // Randomly choose between Heatwave and Snowstorm
                SetWeather(Random.value > 0.5f ? WeatherType.Heatwave : WeatherType.Snowstorm);
                return;
            }
            // If we're in the same adverse weather too long, switch to normal or opposite

            // 70% chance to go back to normal, 30% to opposite extreme
            if (Random.value < 0.7f)
                SetWeather(WeatherType.Normal);
            else
                // Switch to the opposite adverse weather
                SetWeather(CurrentWeather == WeatherType.Heatwave ? WeatherType.Snowstorm : WeatherType.Heatwave);
            return;
        }

        // Force normal weather if we've had too many consecutive adverse events
        if (consecutiveAdverseWeatherCount >= maxConsecutiveAdverseWeather)
        {
            SetWeather(WeatherType.Normal);
            return;
        }

        // Check if we should provide a relaxation period
        if (Random.value < relaxationChance && CurrentWeather != WeatherType.Normal)
        {
            SetWeather(WeatherType.Normal);
            // Extend the normal weather duration
            nextWeatherChangeTime = Time.time + baseWeatherChangeInterval * relaxationDurationMultiplier;
            return;
        }

        if (CurrentWeather == WeatherType.Normal)
        {
            // If player is wearing winter clothing, challenge with heatwave
            if (playerTemperature != null && playerTemperature.currentClothing == ClothingType.WinterCoat)
            {
                // Higher chance of heatwave (80%) if player is well-adapted to cold
                var heatwaveChance = playerAdaptationScore > adaptationThreshold ? 0.8f : 0.5f;

                if (Random.value < heatwaveChance)
                    SetWeather(WeatherType.Heatwave);
                else
                    SetWeather(WeatherType.Snowstorm);
            }
            // If player is wearing summer clothing, challenge with snowstorm
            else if (playerTemperature != null && playerTemperature.currentClothing == ClothingType.TShirt)
            {
                // Higher chance of snowstorm (80%) if player is well-adapted to heat
                var snowstormChance = playerAdaptationScore > adaptationThreshold ? 0.8f : 0.5f;

                if (Random.value < snowstormChance)
                    SetWeather(WeatherType.Snowstorm);
                else
                    SetWeather(WeatherType.Heatwave);
            }
            // If no specific clothing or we don't know, use standard randomization
            else
            {
                // 50/50 chance for Heatwave vs Snowstorm
                if (Random.value > 0.5f)
                    SetWeather(WeatherType.Heatwave);
                else
                    SetWeather(WeatherType.Snowstorm);
            }
        }
        else
        {
            // Decide whether to return to normal or switch to the opposite extreme
            var randomValue = Random.value;

            // Most of the time, return to normal (70%)
            if (randomValue < 0.7f)
            {
                SetWeather(WeatherType.Normal);
            }
            // Sometimes, switch to the opposite extreme (30%)
            else
            {
                if (CurrentWeather == WeatherType.Heatwave)
                    SetWeather(WeatherType.Snowstorm);
                else
                    SetWeather(WeatherType.Heatwave);
            }
        }
    }

    // =================== Private Methods ===================

    /// <summary>
    ///     Monitors how well the player is adapting to current weather conditions.
    /// </summary>
    private void MonitorPlayerAdaptation()
    {
        // Track when clothing changes
        if (lastPlayerClothing != playerTemperature.currentClothing)
            lastPlayerClothing = playerTemperature.currentClothing;

        // Calculate adaptation score (0-1) based on how close to neutral temperature
        var temperatureRange = playerTemperature.maxTemperature - playerTemperature.minTemperature;
        var optimalTemperature = 0f; // Assuming 0 is neutral

        // Calculate how far from optimal (normalized 0-1, where 0 is perfect)
        var deviationFromOptimal = Mathf.Abs(playerTemperature.bodyTemperature - optimalTemperature) /
                                   (temperatureRange * 0.5f);

        // Invert so 1 is perfect adaptation, 0 is poor adaptation
        playerAdaptationScore = 1f - Mathf.Clamp01(deviationFromOptimal);
    }

    /// <summary>
    ///     Displays weather notification text and starts a fade-out after a delay.
    /// </summary>
    private void DisplayWeatherNotification(WeatherType weatherType)
    {
        if (weatherNotificationText == null) return;

        // Stop any existing fade-out
        // if (notificationFadeCoroutine != null)
        // {
        //     StopCoroutine(notificationFadeCoroutine);
        //     notificationFadeCoroutine = null;
        // }

        // Reset notification text alpha to fully visible
        // var color = weatherNotificationText.color;
        // weatherNotificationText.color = new Color(color.r, color.g, color.b, 1f);

        // Set message and color based on weather type
        switch (weatherType)
        {
            case WeatherType.Normal:
                weatherNotificationText.text = "Normal";
                weatherNotificationText.color = new Color(0f, 1f, 0f);
                break;
            case WeatherType.Heatwave:
                weatherNotificationText.text = "Heatwave";
                weatherNotificationText.color = new Color(1f, 0.2745098f, 0f);
                break;
            case WeatherType.Snowstorm:
                weatherNotificationText.text = "Snowstorm";
                weatherNotificationText.color = new Color(0f, 1f, 1f);
                break;
        }

        // Make notification visible
        // weatherNotificationText.gameObject.SetActive(true);
        // weatherNotificationBackground.gameObject.SetActive(true);

        // Start fade out after a delay
        // notificationFadeCoroutine = FadeOutNotification(notificationDisplayTime, fadeDurationText);
        // StartCoroutine(notificationFadeCoroutine);
    }

    /// <summary>
    ///     Plays the appropriate audio clip for the new weather type and smoothly fades it in.
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
        if (weatherAudioSource.isPlaying) weatherAudioSource.Stop();

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
    ///     Toggles visual effects based on the current weather type.
    /// </summary>
    private void ApplyWeatherEffects()
    {
        // Turn off adverse weather effects by default
        if (globalSnowController) globalSnowController.SetVisible(false);
        if (playerSnowController) playerSnowController.SetVisible(false);
        if (fogEffectController) fogEffectController.SetVisible(false);

        // Enable the one relevant effects
        switch (CurrentWeather)
        {
            case WeatherType.Heatwave:
                //heatwave weather effects
                fogEffectController?.SetVisible(true);
                break;
            case WeatherType.Snowstorm:
                //snowstorm weather effects
                globalSnowController?.SetVisible(true);
                playerSnowController?.SetVisible(true);
                break;
            case WeatherType.Normal:
                // standard weather (no effects)
                break;
        }
    }

    /// <summary>
    ///     Dynamically adjusts weather based on player's adaptive choices.
    ///     Uses game theory principles to provide challenging but fair weather patterns.
    /// </summary>
    private IEnumerator AdaptiveWeatherCycle()
    {
        while (true)
        {
            // Wait until next scheduled weather change
            while (Time.time < nextWeatherChangeTime) yield return null;

            // Change weather strategically
            StrategicWeatherChange();

            // Calculate next change time with variability
            var interval = baseWeatherChangeInterval;

            // More frequent changes if player is adapting well (to increase challenge)
            if (playerAdaptationScore > adaptationThreshold) interval *= 0.8f;

            // Add randomness to prevent predictable patterns
            var randomVariance = Random.Range(-5f, 10f);
            interval += randomVariance;

            // Ensure within min/max bounds
            interval = Mathf.Clamp(interval, minWeatherChangeInterval, maxWeatherChangeInterval);

            // Set next change time
            nextWeatherChangeTime = Time.time + interval;
        }
    }

    // =================== Coroutines ===================
    
    /// <summary>
    ///     Gradually fades an AudioSource from 0 up to full volume over 'duration' seconds.
    /// </summary>
    private IEnumerator FadeInSound(float duration)
    {
        var elapsed = 0f;
        weatherAudioSource.volume = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var newVolume = Mathf.Clamp01(elapsed / duration);
            weatherAudioSource.volume = newVolume;
            yield return null;
        }

        // Ensure we end fully at volume = 1
        weatherAudioSource.volume = 1f;
        soundFadeCoroutine = null;
    }
}