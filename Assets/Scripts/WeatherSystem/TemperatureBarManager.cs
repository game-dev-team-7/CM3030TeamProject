using UnityEngine;
using UnityEngine.UI;

public class TemperatureBarManager : MonoBehaviour
{
    // Pulse and Audio Configuration
    [Header("Pulse and Audio Settings")]
    [SerializeField] private float pulseMultiplier = 1f;
    [SerializeField] private AudioClip heartbeatSound;
    [SerializeField] private float minHeartbeatPitch = 0.75f;
    [SerializeField] private float maxHeartbeatPitch = 1.5f;

    // Temperature Thresholds for Feedback
    [Header("Temperature Feedback Thresholds")]
    [SerializeField] private float coldThreshold = 0.2f;
    [SerializeField] private float normalThreshold = 0.4f;
    [SerializeField] private float warmThreshold = 0.6f;
    [SerializeField] private float overheatedThreshold = 0.8f;

    // Pulse Intensity Configuration
    [Header("Pulse Intensity")]
    [SerializeField] private float minPulseSpeed = 0.5f;
    [SerializeField] private float maxPulseSpeed = 2f;
    [SerializeField] private float pulseAlphaIntensity = 0.5f;

    private AudioSource audioSource;
    private bool heartbeatPlayed = false;

    public Outline temperatureBarOutline;
    public Transform emoticonFreeze;
    public Transform emoticonSneeze;
    public Transform emoticonNormal;
    public Transform emoticonMelt;
    public Transform emoticonOverheat;
    private PlayerTemperature playerTemp;

    private float minX = -280f;
    private float maxX = 280f;
    private float minTemperature;
    private float maxTemperature;

    void Start()
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

    void Update()
    {
        if (playerTemp == null) return;

        float currentTemp = playerTemp.bodyTemperature;
        float normalizedTemp = (currentTemp - minTemperature) / (maxTemperature - minTemperature);
        float newX = Mathf.Lerp(minX, maxX, normalizedTemp);

        // Move only the currently visible emoticon
        Transform activeEmoticon = GetActiveEmoticon();

        if (activeEmoticon != null)
        {
            activeEmoticon.localPosition = new Vector3(newX, activeEmoticon.localPosition.y, activeEmoticon.localPosition.z);
        }
        
        HandleEmoticons(normalizedTemp);
        UpdateOutlineGlow(normalizedTemp);
    }

    void HandleEmoticons(float normalizedTemp)
    {
        if (normalizedTemp < coldThreshold)
        {
            ShowEmoticon(emoticonFreeze);
        }
        else if (normalizedTemp >= coldThreshold && normalizedTemp < normalThreshold)
        {
            ShowEmoticon(emoticonSneeze);
        }
        else if (normalizedTemp >= normalThreshold && normalizedTemp < warmThreshold)
        {
            ShowEmoticon(emoticonNormal);
        }
        else if (normalizedTemp >= warmThreshold && normalizedTemp < overheatedThreshold)
        {
            ShowEmoticon(emoticonMelt);
        }
        else if (normalizedTemp >= overheatedThreshold)
        {
            ShowEmoticon(emoticonOverheat);
        }
    }

    void ShowEmoticon(Transform emoticonToShow)
    {
        emoticonFreeze.gameObject.SetActive(emoticonToShow == emoticonFreeze);
        emoticonSneeze.gameObject.SetActive(emoticonToShow == emoticonSneeze);
        emoticonNormal.gameObject.SetActive(emoticonToShow == emoticonNormal);
        emoticonMelt.gameObject.SetActive(emoticonToShow == emoticonMelt);
        emoticonOverheat.gameObject.SetActive(emoticonToShow == emoticonOverheat);
    }

    Transform GetActiveEmoticon()
    {
        if (emoticonFreeze.gameObject.activeSelf) return emoticonFreeze;
        if (emoticonSneeze.gameObject.activeSelf) return emoticonSneeze;
        if (emoticonMelt.gameObject.activeSelf) return emoticonMelt;
        if (emoticonOverheat.gameObject.activeSelf) return emoticonOverheat;
        return emoticonNormal;
    }

    void UpdateOutlineGlow(float normalizedTemp)
    {
        if (temperatureBarOutline == null || playerTemp == null) return;
        
        Color outlineColor = temperatureBarOutline.effectColor;

        // Only trigger pulse and heartbeat for extreme conditions
        if (normalizedTemp <= coldThreshold || normalizedTemp >= overheatedThreshold)
        {
            float changeRate = Mathf.Abs(playerTemp.bodyTemperature / 60f);
            float pulseSpeed = Mathf.Clamp(changeRate * pulseMultiplier, minPulseSpeed, maxPulseSpeed);

            // Adjust heartbeat pitch based on pulse speed
            audioSource.pitch = Mathf.Lerp(minHeartbeatPitch, maxHeartbeatPitch, (pulseSpeed - minPulseSpeed) / (maxPulseSpeed - minPulseSpeed));

            float alpha = Mathf.PingPong(Time.time * pulseSpeed, pulseAlphaIntensity);
            float alphaValue = Mathf.Lerp(0, 1, alpha);

            // Play heartbeat sound when alpha reaches max
            if (alphaValue >= 0.25f && !heartbeatPlayed)
            {
                if (heartbeatSound != null && audioSource != null)
                {
                    audioSource.Play();
                }
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