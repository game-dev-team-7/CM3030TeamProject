using UnityEngine;

public class MinimapWorldObject : MonoBehaviour
{
    [SerializeField]
    private bool followObject = false; // Determines if this object is followed by the minimap camera.

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