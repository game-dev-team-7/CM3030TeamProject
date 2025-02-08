using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public static MinimapController Instance;

    [SerializeField]
    Vector2 worldSize;

    [SerializeField]
    RectTransform scrollViewRectTransform;

    [SerializeField]
    RectTransform contentRectTransform;

    [SerializeField]
    MinimapIcon minimapIconPrefab;

    [SerializeField]
    float generalIconScale = 1f;

    [SerializeField]
    float customerIconScale = 1f;

    Matrix4x4 transformationMatrix;

    private MinimapIcon followIcon;
    private Vector2 scrollViewDefaultSize;
    private Vector2 scrollViewDefaultPosition;
    private Dictionary<MinimapWorldObject, MinimapIcon> miniMapWorldObjectsLookup = new Dictionary<MinimapWorldObject, MinimapIcon>();
    
    // Public getter to access the lookup dictionary
    public Dictionary<MinimapWorldObject, MinimapIcon> MiniMapWorldObjectsLookup => miniMapWorldObjectsLookup;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Set this instance as the singleton
        Instance = this;

        // Store default size and position of the scroll view
        scrollViewDefaultSize = scrollViewRectTransform.sizeDelta;
        scrollViewDefaultPosition = scrollViewRectTransform.anchoredPosition;
    }

    // Called before the first frame update
    private void Start()
    {
        // Calculate the transformation matrix for world-to-map position mapping
        CalculateTransformationMatrix();
    }

    // Called once per frame
    private void Update()
    {
        // Update minimap icons' positions and rotations
        UpdateMiniMapIcons();

        // Center the map on the currently followed icon (if any)
        CenterMapOnIcon();
    }

    // Registers a world object to appear on the minimap
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

    // Removes a world object from the minimap
    public void RemoveMinimapWorldObject(MinimapWorldObject minimapWorldObject)
    {
        // Check if the object is in the lookup dictionary
        if (miniMapWorldObjectsLookup.TryGetValue(minimapWorldObject, out MinimapIcon icon))
        {
            // Remove the object and destroy its associated icon
            if (icon != null && icon.gameObject != null)
            {
                Destroy(icon.gameObject);
            }
            miniMapWorldObjectsLookup.Remove(minimapWorldObject);
        }
    }

    // Centers the minimap on the currently followed icon
    private void CenterMapOnIcon()
    {
        if (followIcon != null)
        {
            float mapScale = contentRectTransform.transform.localScale.x;

            // Move the map opposite to the followed icon's position, scaled by the map's scale
            contentRectTransform.anchoredPosition = (-followIcon.RectTransform.anchoredPosition * mapScale);
        }
    }

    // Updates the positions, rotations, and scales of all minimap icons
    private void UpdateMiniMapIcons()
    {   


        float fixedDistance = 80f; // Distance from player where CustomerLocation icons should appear

        // Find the player's world position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return; // Exit if no player is found
        Vector2 playerMapPosition = WorldPositionToMapPosition(player.transform.position);

        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var miniMapIcon = kvp.Value;

            // Convert world position to minimap position
            Vector2 objectMapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);

            // Distance beteen player and object
            float distancePlayerToObject = Vector2.Distance(playerMapPosition, objectMapPosition);

            if (miniMapWorldObject.tag == "CustomerLocation" && distancePlayerToObject > fixedDistance)
            {
        
                // Calculate direction from player to object
                Vector2 direction = (objectMapPosition - playerMapPosition).normalized;
        
                // Adjust position based on tag
                Vector2 adjustedPosition = objectMapPosition;
                adjustedPosition = playerMapPosition + direction * fixedDistance;

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
            if (miniMapWorldObject.tag == "CustomerLocation")
            {
                // Scale icons inversely to the map scale to maintain consistent size
                float iconScale = customerIconScale / contentRectTransform.transform.localScale.x;            
                miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;

            }
            else
            {
                // Scale icons inversely to the map scale to maintain consistent size
                float iconScale = generalIconScale / contentRectTransform.transform.localScale.x;
                miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;
            }


        }
    }

    // Converts a world position to a minimap position
    private Vector2 WorldPositionToMapPosition(Vector3 worldPos)
    {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return transformationMatrix.MultiplyPoint3x4(pos);
    }

    // Calculates the transformation matrix for mapping world positions to minimap positions
    private void CalculateTransformationMatrix()
    {
        // Get the size of the minimap and the world
        var minimapSize = contentRectTransform.rect.size;
        var worldSize = new Vector2(this.worldSize.x, this.worldSize.y);

        // Calculate the translation and scale ratio for the transformation
        var translation = new Vector2(0,0);
        var scaleRatio = minimapSize / worldSize;

        // Create the transformation matrix using translation, rotation (identity), and scale
        transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);
    }
}
