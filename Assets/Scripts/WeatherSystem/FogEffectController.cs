using UnityEngine;

/// <summary>
///     Controls the visibility of fog effects in the game.
///     This component allows the game to toggle fog visual effects based on gameplay conditions.
/// </summary>
public class FogEffectController : MonoBehaviour
{
    [Header("Fog Effect Settings")] [SerializeField]
    private bool visibleOnStart; // Determines if the fog effect is visible when the game starts

    /// <summary>
    ///     Initializes the fog effect with the default visibility setting.
    /// </summary>
    private void Start()
    {
        // Initialize visibility based on the inspector setting
        gameObject.SetActive(visibleOnStart);
    }

    /// <summary>
    ///     Toggles the active state of the fog effect.
    /// </summary>
    /// <param name="visible">True to show the fog effect, false to hide it</param>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);

        // Optional debug message
        Debug.Log("Fog effect " + (visible ? "enabled" : "disabled"));
    }
}