using UnityEngine;

/// <summary>
///     Advanced camera controller that allows for smooth orbital movement around a target.
///     Provides user input handling for camera rotation and smooth follow behavior.
/// </summary>
public class CameraControllerV2 : MonoBehaviour
{
    [Header("Target to follow")] public Transform target;

    [Header("Camera settings")] public float rotationSpeed = 3f; // Speed of camera rotation when using mouse

    public float camDistance = 60f; // Distance from camera to target
    public float camHeight = 20f; // Height offset of camera from target
    public float camHorizontalAngle; // Initial horizontal angle
    public float camVerticalAngle; // Initial vertical angle
    public float smoothTime = 0.1f; // Smoothness factor for camera movement

    // Variables for camera rotation smoothing
    private float currentHorizontal; // Current horizontal angle
    private float currentVertical; // Current vertical angle
    private float horizontalVelocity; // Velocity for horizontal smoothing

    private Vector3 offset; // Offset from target position
    private float smoothHorizontal; // Smoothed horizontal angle
    private float smoothVertical; // Smoothed vertical angle
    private float verticalVelocity; // Velocity for vertical smoothing

    /// <summary>
    ///     Initializes the camera controller with default rotation values and offset.
    /// </summary>
    private void Start()
    {
        currentHorizontal = camHorizontalAngle;
        currentVertical = camVerticalAngle;
        offset = new Vector3(0, camHeight, -camDistance);
    }

    /// <summary>
    ///     Handles user input for camera rotation and cursor visibility.
    ///     Called once per frame.
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButton(1)) // Right mouse button held
        {
            // Hide cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Handle camera rotation logic
            currentHorizontal += Input.GetAxis("Mouse X") * rotationSpeed;
            currentVertical -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentVertical = Mathf.Clamp(currentVertical, -5f, 70f); // Limit vertical angle
        }
        else
        {
            // Show cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Smooth out the camera rotation using Mathf.SmoothDamp
        smoothHorizontal = Mathf.SmoothDamp(smoothHorizontal, currentHorizontal, ref horizontalVelocity, smoothTime);
        smoothVertical = Mathf.SmoothDamp(smoothVertical, currentVertical, ref verticalVelocity, smoothTime);
    }

    /// <summary>
    ///     Updates camera position and rotation after all Update methods are called.
    ///     Ensures smooth camera following and proper positioning.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 5f);

        // Apply smooth rotation
        var rotation = Quaternion.Euler(smoothVertical, smoothHorizontal, 0);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target.position + Vector3.up * (camHeight * 0.5f));
    }
}