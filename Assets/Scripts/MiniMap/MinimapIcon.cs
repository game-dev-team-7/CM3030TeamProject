using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Represents an icon on the minimap UI corresponding to a world object.
///     Provides references to UI components needed for positioning and display.
/// </summary>
public class MinimapIcon : MonoBehaviour
{
    /// <summary>
    ///     Reference to the Image component that displays the minimap icon's sprite.
    /// </summary>
    public Image Image;

    /// <summary>
    ///     Reference to the RectTransform for positioning the icon on the minimap.
    /// </summary>
    public RectTransform RectTransform;

    /// <summary>
    ///     Reference to the RectTransform specifically for the icon itself (useful for scaling/rotation).
    /// </summary>
    public RectTransform IconRectTransform;
}