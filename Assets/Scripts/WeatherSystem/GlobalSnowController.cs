using UnityEngine;

/// <summary>
///     Controls the visibility of global snow effects in the game.
///     This component allows the game to toggle global snow visual effects based on weather conditions.
/// </summary>
public class GlobalSnowController : MonoBehaviour
{
    [Header("Snow Effect Settings")] [SerializeField]
    private bool visibleOnStart; // Determines if the snow effect is visible when the game starts

    /// <summary>
    ///     Initializes the snow effect with the default visibility setting.
    /// </summary>
    private void Start()
    {
        // Initialize visibility based on the inspector setting
        gameObject.SetActive(visibleOnStart);
    }

    /// <summary>
    ///     Toggles the active state of the global snow effect.
    /// </summary>
    /// <param name="visible">True to show the snow effect, false to hide it</param>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);

        // Optional debug message
        Debug.Log("Global snow effect " + (visible ? "enabled" : "disabled"));
    }
}