using UnityEngine;

/// <summary>
///     Controls the toggling of a spotlight object, typically used for vehicle headlights.
///     Allows the player to turn the light on and off with a keypress.
/// </summary>
public class LightToggle : MonoBehaviour
{
    [Header("Light References")]
    public GameObject spotLightObject; // Reference to the GameObject containing the Light component

    private Light spotLight; // Reference to the actual Light component

    /// <summary>
    ///     Initializes the light toggle component by getting the Light component from the provided GameObject.
    /// </summary>
    private void Start()
    {
        if (spotLightObject != null)
        {
            spotLight = spotLightObject.GetComponent<Light>(); // Get the Light component

            if (spotLight == null) Debug.LogWarning("LightToggle: No Light component found on the spotLightObject!");
        }
        else
        {
            Debug.LogWarning("LightToggle: No spotLightObject assigned!");
        }
    }

    /// <summary>
    ///     Checks for player input to toggle the light on and off.
    ///     Called once per frame to respond to player input.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && spotLight != null)
        {
            spotLight.enabled = !spotLight.enabled; // Toggle the light on/off
            Debug.Log("Light toggled: " + (spotLight.enabled ? "ON" : "OFF"));
        }
    }
}