using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Manages enhanced visual and audio feedback for the temperature system.
///     Provides dynamic UI elements like emoticons, pulsing effects, and heartbeat sounds
///     to represent the player's current temperature status.
/// </summary>
public class TemperatureBarManager : MonoBehaviour
{
    // Pulse and Audio Configuration
    [Header("Pulse and Audio Settings")] [SerializeField]
    private float pulseMultiplier = 1f;

    [SerializeField] private AudioClip heartbeatSound;
    [SerializeField] private float minHeartbeatPitch = 0.75f;
    [SerializeField] private float maxHeartbeatPitch = 1.5f;

    // Temperature Thresholds for Feedback
    [Header("Temperature Feedback Thresholds")] [SerializeField]
    private float coldThreshold = 0.2f;

    [SerializeField] private float normalThreshold = 0.4f;
    [SerializeField] private float warmThreshold = 0.6f;
    [SerializeField] private float overheatedThreshold = 0.8f;

    // Pulse Intensity Configuration
    [Header("Pulse Intensity")] [SerializeField]
    private float minPulseSpeed = 0.5f;

    [SerializeField] private float maxPulseSpeed = 2f;
    [SerializeField] private float pulseAlphaIntensity = 0.5f;

    [Header("UI References")] public Outline temperatureBarOutline;

    public Transform emoticonFreeze;
    public Transform emoticonSneeze;
    public Transform emoticonNormal;
    public Transform emoticonMelt;
    public Transform emoticonOverheat;

    private AudioSource audioSource;
    private bool heartbeatPlayed;
    private float maxTemperature;
    private readonly float maxX = 280f;
    private float minTemperature;

    // Position limits for emoticon movement
    private readonly float minX = -280f;

    private PlayerTemperature playerTemp;

    /// <summary>
    ///     Initializes the temperature bar manager, finding required components and setting up audio.
    /// </summary>
    private void Start()
    {
        // Find the Player object that has the PlayerTemperature script
        playerTemp = FindObjectOfType<PlayerTemperature>();

        if (playerTemp == null)
        {
            Debug.LogError("PlayerTemperature script not found in the scene!");
        }
        else
        {
            minTemperature = playerTemp.minTemperature;
            maxTemperature = playerTemp.maxTemperature;
        }

        ShowEmoticon(emoticonNormal);

        // Setup AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = heartbeatSound;
    }

    /// <summary>
    ///     Updates the temperature visualization based on the current player temperature.
    /// </summary>
    private void Update()
    {
        if (playerTemp == null) return;

        var currentTemp = playerTemp.bodyTemperature;
        var normalizedTemp = (currentTemp - minTemperature) / (maxTemperature - minTemperature);
        var newX = Mathf.Lerp(minX, maxX, normalizedTemp);

        // Move only the currently visible emoticon
        var activeEmoticon = GetActiveEmoticon();

        if (activeEmoticon != null)
            activeEmoticon.localPosition =
                new Vector3(newX, activeEmoticon.localPosition.y, activeEmoticon.localPosition.z);

        HandleEmoticons(normalizedTemp);
        UpdateOutlineGlow(normalizedTemp);
    }

    /// <summary>
    ///     Determines which emoticon to display based on the current temperature.
    /// </summary>
    /// <param name="normalizedTemp">The normalized temperature value (0-1)</param>
    private void HandleEmoticons(float normalizedTemp)
    {
        if (normalizedTemp < coldThreshold)
            ShowEmoticon(emoticonFreeze);
        else if (normalizedTemp >= coldThreshold && normalizedTemp < normalThreshold)
            ShowEmoticon(emoticonSneeze);
        else if (normalizedTemp >= normalThreshold && normalizedTemp < warmThreshold)
            ShowEmoticon(emoticonNormal);
        else if (normalizedTemp >= warmThreshold && normalizedTemp < overheatedThreshold)
            ShowEmoticon(emoticonMelt);
        else if (normalizedTemp >= overheatedThreshold) ShowEmoticon(emoticonOverheat);
    }

    /// <summary>
    ///     Shows only the specified emoticon and hides all others.
    /// </summary>
    /// <param name="emoticonToShow">The emoticon transform to show</param>
    private void ShowEmoticon(Transform emoticonToShow)
    {
        emoticonFreeze.gameObject.SetActive(emoticonToShow == emoticonFreeze);
        emoticonSneeze.gameObject.SetActive(emoticonToShow == emoticonSneeze);
        emoticonNormal.gameObject.SetActive(emoticonToShow == emoticonNormal);
        emoticonMelt.gameObject.SetActive(emoticonToShow == emoticonMelt);
        emoticonOverheat.gameObject.SetActive(emoticonToShow == emoticonOverheat);
    }

    /// <summary>
    ///     Gets the currently active emoticon transform.
    /// </summary>
    /// <returns>The transform of the active emoticon</returns>
    private Transform GetActiveEmoticon()
    {
        if (emoticonFreeze.gameObject.activeSelf) return emoticonFreeze;
        if (emoticonSneeze.gameObject.activeSelf) return emoticonSneeze;
        if (emoticonMelt.gameObject.activeSelf) return emoticonMelt;
        if (emoticonOverheat.gameObject.activeSelf) return emoticonOverheat;
        return emoticonNormal;
    }

    /// <summary>
    ///     Updates the temperature bar outline glow effect based on current temperature.
    ///     Creates a pulsing effect and plays heartbeat sounds for extreme temperatures.
    /// </summary>
    /// <param name="normalizedTemp">The normalized temperature value (0-1)</param>
    private void UpdateOutlineGlow(float normalizedTemp)
    {
        if (temperatureBarOutline == null || playerTemp == null) return;

        var outlineColor = temperatureBarOutline.effectColor;

        // Only trigger pulse and heartbeat for extreme conditions
        if (normalizedTemp <= coldThreshold || normalizedTemp >= overheatedThreshold)
        {
            var changeRate = Mathf.Abs(playerTemp.bodyTemperature / 60f);
            var pulseSpeed = Mathf.Clamp(changeRate * pulseMultiplier, minPulseSpeed, maxPulseSpeed);

            // Adjust heartbeat pitch based on pulse speed
            audioSource.pitch = Mathf.Lerp(minHeartbeatPitch, maxHeartbeatPitch,
                (pulseSpeed - minPulseSpeed) / (maxPulseSpeed - minPulseSpeed));

            var alpha = Mathf.PingPong(Time.time * pulseSpeed, pulseAlphaIntensity);
            var alphaValue = Mathf.Lerp(0, 1, alpha);

            // Play heartbeat sound when alpha reaches max
            if (alphaValue >= 0.25f && !heartbeatPlayed)
            {
                if (heartbeatSound != null && audioSource != null) audioSource.Play();
                heartbeatPlayed = true;
            }
            else if (alphaValue < 0.25f)
            {
                heartbeatPlayed = false;
            }

            // Apply new alpha to the outline color
            temperatureBarOutline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, alphaValue);
        }
        else
        {
            // No pulse or sound for moderate temperatures
            temperatureBarOutline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0);
        }
    }
}