using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Controls the minimap system, handling registration, positioning and scaling of icons.
///     Acts as a singleton for easy access from other scripts.
/// </summary>
public class MinimapController : MonoBehaviour
{
    /// <summary>
    ///     Singleton instance of the MinimapController.
    /// </summary>
    public static MinimapController Instance;

    /// <summary>
    ///     Size of the game world in world units (width, height).
    /// </summary>
    [SerializeField] private Vector2 worldSize;

    /// <summary>
    ///     Reference to the scroll view containing the minimap.
    /// </summary>
    [SerializeField] private RectTransform scrollViewRectTransform;

    /// <summary>
    ///     Reference to the content area within the scroll view that contains the minimap icons.
    /// </summary>
    [SerializeField] private RectTransform contentRectTransform;

    /// <summary>
    ///     Prefab used to instantiate new minimap icons.
    /// </summary>
    [SerializeField] private MinimapIcon minimapIconPrefab;

    /// <summary>
    ///     Reference to the camera pivot for determining view direction.
    /// </summary>
    [SerializeField] private Transform cameraPivot;

    /// <summary>
    ///     UI element representing the camera's direction on the minimap.
    /// </summary>
    [SerializeField] private RectTransform cameraDirectionLight;

    /// <summary>
    ///     Scale factor for general minimap icons.
    /// </summary>
    [SerializeField] private float generalIconScale = 1f;

    /// <summary>
    ///     Scale factor for customer minimap icons.
    /// </summary>
    [SerializeField] private float customerIconScale = 1f;

    /// <summary>
    ///     The icon that the minimap is currently centered on.
    /// </summary>
    private MinimapIcon followIcon;

    /// <summary>
    ///     Dictionary mapping world objects to their corresponding minimap icons.
    /// </summary>
    private readonly Dictionary<MinimapWorldObject, MinimapIcon> miniMapWorldObjectsLookup = new();

    /// <summary>
    ///     Default position of the scroll view.
    /// </summary>
    private Vector2 scrollViewDefaultPosition;

    /// <summary>
    ///     Default size of the scroll view.
    /// </summary>
    private Vector2 scrollViewDefaultSize;

    /// <summary>
    ///     Matrix used to transform world coordinates to minimap coordinates.
    /// </summary>
    private Matrix4x4 transformationMatrix;

    /// <summary>
    ///     Public getter to access the lookup dictionary of world objects to minimap icons.
    /// </summary>
    public Dictionary<MinimapWorldObject, MinimapIcon> MiniMapWorldObjectsLookup => miniMapWorldObjectsLookup;

    /// <summary>
    ///     Called when the script instance is being loaded.
    ///     Initializes the singleton instance and stores default UI values.
    /// </summary>
    private void Awake()
    {
        // Set this instance as the singleton
        Instance = this;

        // Store default size and position of the scroll view
        scrollViewDefaultSize = scrollViewRectTransform.sizeDelta;
        scrollViewDefaultPosition = scrollViewRectTransform.anchoredPosition;
    }

    /// <summary>
    ///     Called before the first frame update.
    ///     Calculates the world-to-minimap transformation matrix.
    /// </summary>
    private void Start()
    {
        // Calculate the transformation matrix for world-to-map position mapping
        CalculateTransformationMatrix();
    }

    /// <summary>
    ///     Called once per frame.
    ///     Updates minimap icon positions and centers the map if needed.
    /// </summary>
    private void Update()
    {
        // Update minimap icons' positions and rotations
        UpdateMiniMapIcons();

        // Center the map on the currently followed icon (if any)
        CenterMapOnIcon();
    }

    /// <summary>
    ///     Registers a world object to appear on the minimap.
    /// </summary>
    /// <param name="miniMapWorldObject">The world object to register.</param>
    /// <param name="followObject">Whether the minimap should follow this object.</param>
    public void RegisterMinimapWorldObject(MinimapWorldObject miniMapWorldObject, bool followObject = false)
    {
        // Create a minimap icon for the world object
        var minimapIcon = Instantiate(minimapIconPrefab);
        minimapIcon.transform.SetParent(contentRectTransform);

        // Set the icon's sprite and store it in the lookup dictionary
        minimapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;
        miniMapWorldObjectsLookup[miniMapWorldObject] = minimapIcon;

        // Set the icon to be followed if specified
        if (followObject)
            followIcon = minimapIcon;
    }

    /// <summary>
    ///     Removes a world object from the minimap.
    /// </summary>
    /// <param name="minimapWorldObject">The world object to remove.</param>
    public void RemoveMinimapWorldObject(MinimapWorldObject minimapWorldObject)
    {
        // Check if the object is in the lookup dictionary
        if (miniMapWorldObjectsLookup.TryGetValue(minimapWorldObject, out var icon))
        {
            // Remove the object and destroy its associated icon
            if (icon != null && icon.gameObject != null) Destroy(icon.gameObject);
            miniMapWorldObjectsLookup.Remove(minimapWorldObject);
        }
    }

    /// <summary>
    ///     Centers the minimap on the currently followed icon.
    /// </summary>
    private void CenterMapOnIcon()
    {
        if (followIcon != null)
        {
            var mapScale = contentRectTransform.transform.localScale.x;

            // Move the map opposite to the followed icon's position, scaled by the map's scale
            contentRectTransform.anchoredPosition = -followIcon.RectTransform.anchoredPosition * mapScale;
        }
    }

    /// <summary>
    ///     Updates the positions, rotations, and scales of all minimap icons.
    /// </summary>
    private void UpdateMiniMapIcons()
    {
        // Find the player's world position
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return; // Exit if no player is found
        var playerMapPosition = WorldPositionToMapPosition(player.transform.position);

        // Update cameraDirectionLight's position and rotation
        if (cameraDirectionLight != null && cameraPivot != null)
        {
            var cameraDirectionRect = cameraDirectionLight.GetComponent<RectTransform>();
            if (cameraDirectionRect != null)
            {
                // Set the position to the player's minimap position
                cameraDirectionRect.anchoredPosition = playerMapPosition;

                // Get Y-axis rotation from cameraPivot
                var cameraPivotYRotation = cameraPivot.transform.eulerAngles.y;

                // Apply rotation (negate it to match minimap's 2D Z-axis)
                cameraDirectionRect.localRotation = Quaternion.Euler(0, 0, -cameraPivotYRotation);
            }
        }

        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var miniMapIcon = kvp.Value;

            // Convert world position to minimap position
            var objectMapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);

            // Distance between player and object
            var distancePlayerToObject = Vector2.Distance(playerMapPosition, objectMapPosition);

            if (miniMapWorldObject.alwaysShow && distancePlayerToObject > miniMapWorldObject.minimapDistanceFromPlayer)
            {
                // Calculate player to object direction
                var direction = (objectMapPosition - playerMapPosition).normalized;

                // Adjust minimap distance from player to customer
                var adjustedPosition = playerMapPosition + direction * miniMapWorldObject.minimapDistanceFromPlayer;

                // Update the icon's minimap position
                miniMapIcon.RectTransform.anchoredPosition = adjustedPosition;

                // Rotate icon to point toward object
                miniMapIcon.IconRectTransform.localRotation = Quaternion.FromToRotation(Vector2.up, -direction);
            }
            else
            {
                // Update the icon's position, rotation, and scale
                miniMapIcon.RectTransform.anchoredPosition = objectMapPosition;
                var rotation = miniMapWorldObject.transform.rotation.eulerAngles;
                miniMapIcon.IconRectTransform.localRotation = Quaternion.AngleAxis(-rotation.y, Vector3.forward);
            }

            // Scale the icon
            if (miniMapWorldObject.CompareTag("Customer"))
            {
                // Scale icons inversely to the map scale to maintain consistent size
                var iconScale = customerIconScale / contentRectTransform.transform.localScale.x;
                miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;
            }
            else
            {
                // Scale icons inversely to the map scale to maintain consistent size
                var iconScale = generalIconScale / contentRectTransform.transform.localScale.x;
                miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;
            }
        }
    }

    /// <summary>
    ///     Converts a world position to a minimap position.
    /// </summary>
    /// <param name="worldPos">The position in world space.</param>
    /// <returns>The corresponding position on the minimap.</returns>
    private Vector2 WorldPositionToMapPosition(Vector3 worldPos)
    {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return transformationMatrix.MultiplyPoint3x4(pos);
    }

    /// <summary>
    ///     Calculates the transformation matrix for mapping world positions to minimap positions.
    /// </summary>
    private void CalculateTransformationMatrix()
    {
        // Get the size of the minimap and the world
        var minimapSize = contentRectTransform.rect.size;
        var worldSize = new Vector2(this.worldSize.x, this.worldSize.y);

        // Calculate the translation and scale ratio for the transformation
        var translation = new Vector2(0, 0);
        var scaleRatio = minimapSize / worldSize;

        // Create the transformation matrix using translation, rotation (identity), and scale
        transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);
    }
}