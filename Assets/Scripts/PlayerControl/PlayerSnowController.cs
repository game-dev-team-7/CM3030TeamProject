using UnityEngine;

/// <summary>
///     Controls the visibility of snow effects around the player.
///     This component allows the game to toggle snow visual effects based on gameplay conditions.
/// </summary>
public class PlayerSnowController : MonoBehaviour
{
    [Header("Snow Effect Settings")] [SerializeField]
    private bool visibleOnStart = true; // Determines if the snow effect is visible when the game starts

    /// <summary>
    ///     Initializes the snow effect with the default visibility setting.
    /// </summary>
    private void Start()
    {
        // Initialize visibility based on the inspector setting
        gameObject.SetActive(visibleOnStart);
    }

    /// <summary>
    ///     Toggles the active state of the snow effect around the player.
    /// </summary>
    /// <param name="visible">True to show the snow effect, false to hide it</param>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);

        // Optional debug message
        Debug.Log("Snow effect " + (visible ? "enabled" : "disabled"));
    }
}