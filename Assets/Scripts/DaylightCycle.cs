using UnityEngine;

/// <summary>
///     Controls the rotation of the directional light to simulate day and night cycles.
///     Creates a realistic time progression by rotating the sun around the scene.
/// </summary>
public class DaylightCycle : MonoBehaviour
{
    [Header("Cycle Settings")] [Tooltip("The total time for a complete day/night cycle in seconds")] [SerializeField]
    private float totalCycleTime = 180.0f; // 3 minutes for a complete cycle

    [Tooltip("The total rotation angle from sunrise to sunset in degrees")] [SerializeField]
    private float totalRotationAngle = 180.0f; // 180 degrees rotation (half circle)

    private float degreesPerSecond; // Calculated rotation speed

    private Vector3 rotationAmount = Vector3.zero; // The amount to rotate each frame

    /// <summary>
    ///     Initializes the daylight cycle system by calculating the rotation speed.
    /// </summary>
    private void Start()
    {
        // Calculate the rotation speed (degrees per second)
        degreesPerSecond = totalRotationAngle / totalCycleTime;

        Debug.Log($"DaylightCycle initialized: {degreesPerSecond} degrees per second");
    }

    /// <summary>
    ///     Updates the rotation of the directional light each frame to simulate sun movement.
    ///     Called once per frame.
    /// </summary>
    private void Update()
    {
        // Calculate rotation amount for this frame
        rotationAmount.x = degreesPerSecond * Time.deltaTime;

        // Apply rotation around the world X axis
        transform.Rotate(rotationAmount, Space.World);
    }
}