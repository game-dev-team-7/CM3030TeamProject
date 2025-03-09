using UnityEngine;

/// <summary>
///     Component that marks a GameObject to be displayed on the minimap.
///     Handles registration and cleanup with the MinimapController.
/// </summary>
public class MinimapWorldObject : MonoBehaviour
{
    /// <summary>
    ///     Determines if this object is followed by the minimap camera.
    /// </summary>
    [SerializeField] private bool followObject;

    /// <summary>
    ///     Determines if this object is always shown on minimap regardless of distance.
    /// </summary>
    [SerializeField] public bool alwaysShow;

    /// <summary>
    ///     Maximum distance from player where object should be shown on the minimap edge.
    /// </summary>
    [SerializeField] public float minimapDistanceFromPlayer = 90f;

    /// <summary>
    ///     The icon representing this object on the minimap.
    /// </summary>
    [SerializeField] private Sprite minimapIcon;

    /// <summary>
    ///     Public getter for the minimap icon.
    /// </summary>
    public Sprite MinimapIcon => minimapIcon;

    /// <summary>
    ///     Called when the object is enabled.
    ///     Registers this object with the MinimapController.
    /// </summary>
    private void Start()
    {
        // Verify that the MinimapController instance exists before registering
        if (MinimapController.Instance != null)
            // Registers this object with the MinimapController, optionally setting it to be followed
            MinimapController.Instance.RegisterMinimapWorldObject(this, followObject);
        else
            Debug.LogWarning("MinimapController instance not found. Unable to register " + gameObject.name);
    }

    /// <summary>
    ///     Called when the object is destroyed.
    ///     Removes this object from the MinimapController.
    /// </summary>
    private void OnDestroy()
    {
        // Verify that the MinimapController instance exists before removing
        if (MinimapController.Instance != null)
            // Removes this object from the MinimapController when it is destroyed
            MinimapController.Instance.RemoveMinimapWorldObject(this);
    }
}