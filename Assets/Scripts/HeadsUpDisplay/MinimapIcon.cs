using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    // Reference to the Image component that displays the minimap icon's sprite
    public Image Image;

    // Reference to the RectTransform for positioning the icon on the minimap
    public RectTransform RectTransform;

    // Reference to the RectTransform specifically for the icon itself (useful for scaling/rotation)
    public RectTransform IconRectTransform;
}