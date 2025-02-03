using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float rotationSpeed = 3f;

    [Header("Default Position")] public float defaultDistance = 67.4f;
    public float defaultHeight = 83.1f;
    public float defaultHorizontalAngle = 0f;
    public float defaultVerticalAngle = 7f;

    private float currentHorizontal;
    private float currentVertical;
    private Vector3 offset;
    private Vector3 velocity;

    /// <summary>
    /// Initializes the camera controller.
    /// 
    /// Sets the current horizontal and vertical angles to their default values.
    /// Also sets the camera's offset based on the default distance, height, and angles.
    /// </summary>
    private void Start()
    {
        currentHorizontal = defaultHorizontalAngle;
        currentVertical = defaultVerticalAngle;
        offset = new Vector3(0, defaultHeight, -defaultDistance);
    }

    /// <summary>
    /// Smoothly follows the target's position and handles camera rotation input.
    /// 
    /// If the target is null, this function does nothing.
    /// 
    /// Smoothly follows the target's position using SmoothDamp.
    /// 
    /// Handles camera rotation input by listening for the right mouse button being
    /// held down. When the right mouse button is held down, the camera rotates
    /// according to the mouse's horizontal and vertical input.
    /// 
    /// Applies the calculated rotations to the camera.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        // Smoothly follow the player's position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            target.position,
            ref velocity,
            0.1f // Adjust smooth time
        );

        // Handle camera rotation input
        if (Input.GetMouseButton(1)) // Right mouse button held
        {
            currentHorizontal += Input.GetAxis("Mouse X") * rotationSpeed;
            currentVertical -= Input.GetAxis("Mouse Y") * rotationSpeed;
        }

        // Apply rotations
        var rotation = Quaternion.Euler(currentVertical, currentHorizontal, 0);
        Camera.main.transform.localPosition = rotation * offset;
        Camera.main.transform.LookAt(target.position + Vector3.up * (defaultHeight * 0.5f));
    }
}