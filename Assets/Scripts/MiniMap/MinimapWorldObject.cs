using UnityEngine;

public class MinimapWorldObject : MonoBehaviour
{
    [SerializeField]
    private bool followObject = false; // Determines if this object is followed by the minimap camera.

    [SerializeField]
    public bool alwaysShow = false; // Determines if this object is always shown on minimap

    [SerializeField]
    public float minimapDistanceFromPlayer = 90f; // Distance from player where object should always show

    [SerializeField]
    private Sprite minimapIcon; // The icon representing this object on the minimap.

    // Public getter for the minimap icon.
    public Sprite MinimapIcon => minimapIcon;

    private void Start()
    {
        // Registers this object with the MinimapController, optionally setting it to be followed.
        MinimapController.Instance.RegisterMinimapWorldObject(this, followObject);
    }

    private void OnDestroy()
    {
        // Removes this object from the MinimapController when it is destroyed.
        MinimapController.Instance.RemoveMinimapWorldObject(this);
    }


}