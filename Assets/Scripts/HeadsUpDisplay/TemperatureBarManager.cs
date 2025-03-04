using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureBarManager : MonoBehaviour
{
    [SerializeField] private float pulseMultiplier = 1f; // Adjust pulsation speed
    [SerializeField] private AudioClip heartbeatSound; // Assign heartbeat sound
    [SerializeField] private float minHeartbeatPitch = 0.75f; // Slowest pitch
    [SerializeField] private float maxHeartbeatPitch = 1.5f; // Fastest pitch

    private AudioSource audioSource;
    private bool heartbeatPlayed =false; // Prevent multiple plays per cycle

    public Outline temperatureBarOutline; //Outline for temperature bar glow effect
    public Transform emoticonFreeze;    // For freezing cold
    public Transform emoticonSneeze;    // For cold
    public Transform emoticonNormal;    // Default emoticon
    public Transform emoticonMelt;      // For hot
    public Transform emoticonOverheat;  // For overheating hot
    private PlayerTemperature playerTemp; // Reference to PlayerTemperature

    private float minX = -280f; // Leftmost position on the bar
    private float maxX = 280f;  // Rightmost position on the bar
    private float minTemperature;
    private float maxTemperature;

    void Start()
    {
        // Find the Player object that has the PlayerTemperature script
        playerTemp = FindObjectOfType<PlayerTemperature>();

        if (playerTemp == null)
        {
            Debug.LogError("PlayerTemperature script not found in the scene!");
        } else {
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

        float currentTemp = playerTemp.bodyTemperature; // Get the body temperature
        float normalizedTemp = (currentTemp - minTemperature) / (maxTemperature - minTemperature); // Normalize bodyTemperature to range (0 to 1)
        float newX = Mathf.Lerp(minX, maxX, normalizedTemp); // Map to X-axis range

        // Move only the currently visible emoticon
        Transform activeEmoticon = GetActiveEmoticon();

        if (activeEmoticon != null)
        {
            activeEmoticon.localPosition = new Vector3(newX, activeEmoticon.localPosition.y, activeEmoticon.localPosition.z);
        }

        
        HandleEmoticons(normalizedTemp);
        UpdateOutlineGlow(currentTemp);
    }

    // Handle which emoticon is shown
    void HandleEmoticons(float normalizedTemp)
    {
        if (normalizedTemp < 0.2f)
        {
            ShowEmoticon(emoticonFreeze);
        }
        else if (normalizedTemp > 0.2f && normalizedTemp < 0.4f)
        {
            ShowEmoticon(emoticonSneeze);
        }
        else if (normalizedTemp > 0.4f && normalizedTemp < 0.6f)
        {
            ShowEmoticon(emoticonNormal);
        }
        else if (normalizedTemp > 0.6f && normalizedTemp < 0.8f)
        {
            ShowEmoticon(emoticonMelt);
        }
        else if (normalizedTemp > 0.8f)
        {
            ShowEmoticon(emoticonOverheat);
        }
    }

    // Show only one emoticon at a time
    void ShowEmoticon(Transform emoticonToShow)
    {
        emoticonFreeze.gameObject.SetActive(emoticonToShow == emoticonFreeze);
        emoticonSneeze.gameObject.SetActive(emoticonToShow == emoticonSneeze);
        emoticonNormal.gameObject.SetActive(emoticonToShow == emoticonNormal);
        emoticonMelt.gameObject.SetActive(emoticonToShow == emoticonMelt);
        emoticonOverheat.gameObject.SetActive(emoticonToShow == emoticonOverheat);
    }

    // Find which emoticon is currently active
    Transform GetActiveEmoticon()
    {
        if (emoticonFreeze.gameObject.activeSelf) return emoticonFreeze;
        if (emoticonSneeze.gameObject.activeSelf) return emoticonSneeze;
        if (emoticonMelt.gameObject.activeSelf) return emoticonMelt;
        if (emoticonOverheat.gameObject.activeSelf) return emoticonOverheat;
        return emoticonNormal;
    }

    void UpdateOutlineGlow(float currentTemp)
    {
        if (temperatureBarOutline == null || playerTemp == null) return;
        
        Color outlineColor = temperatureBarOutline.effectColor;
        currentTemp = Mathf.Abs(currentTemp);

        if (currentTemp > 30f)
        {
            float changeRate = Mathf.Abs(playerTemp.bodyTemperature / 60f); // Get the absolute temperature rate per second
            float pulseSpeed = Mathf.Clamp(changeRate * pulseMultiplier, 0.5f, 5f); // Ensure the pulse speed is within a reasonable range

            // Adjust heartbeat pitch based on pulse speed
            audioSource.pitch = Mathf.Lerp(minHeartbeatPitch, maxHeartbeatPitch, (pulseSpeed - 0.5f) / (5f - 0.5f));

            // float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2; // Calculate alpha pulsing effect
            float alpha = Mathf.PingPong(Time.time * pulseSpeed, 1); // Calculate alpha pulsing effect
            float alphaValue  = Mathf.Lerp(0, 1, alpha); // Convert to Unity's 0-1 range

             // Play heartbeat sound when alpha reaches max (1.0)
            if (alphaValue >= 0.25f && !heartbeatPlayed)
            {
                if (heartbeatSound != null && audioSource != null)
                {
                    audioSource.Play();
                }
                heartbeatPlayed = true; // Ensure it plays only once per pulse
            }
            else if (alphaValue < 0.25f)
            {
                heartbeatPlayed = false; // Reset when the pulse fades
            }

            // Apply new alpha to the outline color
            temperatureBarOutline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, alphaValue);
        }
        else {
            temperatureBarOutline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0);
        }

    }
}
