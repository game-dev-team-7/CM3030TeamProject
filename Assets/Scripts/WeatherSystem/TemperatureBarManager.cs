using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureBarManager : MonoBehaviour
{
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
    }

    void Update()
    {
        if (playerTemp == null) return;

        float currentTemp = playerTemp.bodyTemperature; // Get the body temperature
        //Debug.Log("Player's Body Temperature: " + currentTemp);

        // Normalize bodyTemperature to range (0 to 1)
        float normalizedTemp = (currentTemp - minTemperature) / (maxTemperature - minTemperature);

        // Map to X-axis range
        float newX = Mathf.Lerp(minX, maxX, normalizedTemp);

        // Move only the currently visible emoticon
        Transform activeEmoticon = GetActiveEmoticon();

        if (activeEmoticon != null)
        {
            activeEmoticon.localPosition = new Vector3(newX, activeEmoticon.localPosition.y, activeEmoticon.localPosition.z);
        }

        // Handle which emoticon is shown
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
}
