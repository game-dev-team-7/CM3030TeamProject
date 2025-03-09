using UnityEngine;

/// <summary>
///     Controls engine sound pitch and volume based on vehicle speed.
///     Provides realistic audio feedback that corresponds to the vehicle's movement.
/// </summary>
public class EngineSoundControl : MonoBehaviour
{
    [Header("Audio References")] public AudioSource engineSound; // Reference to the vehicle's audio source

    [Header("Vehicle References")] public Rigidbody vehicleRigidbody; // Reference to the vehicle's Rigidbody

    [Header("Sound Settings")] public float minPitch = 0.5f; // Pitch at idle

    public float maxPitch = 2.0f; // Pitch at max speed
    public float minVolume = 0.2f; // Minimum volume when moving
    public float maxVolume = 0.5f; // Maximum volume at high speeds
    public float speedThreshold = 0.5f; // Minimum speed to play sound
    public float maxPitchSpeed = 50f; // Speed at which max pitch is reached
    public float maxVolumeSpeed = 20f; // Speed at which max volume is reached

    /// <summary>
    ///     Initializes the engine sound component.
    ///     Gets required components if not assigned and starts the audio source.
    /// </summary>
    private void Start()
    {
        if (engineSound == null)
        {
            engineSound = GetComponent<AudioSource>(); // Get AudioSource if not assigned
            Debug.Log("EngineSoundControl: AudioSource automatically assigned");
        }

        if (vehicleRigidbody == null)
        {
            vehicleRigidbody = GetComponent<Rigidbody>(); // Get Rigidbody if not assigned
            Debug.Log("EngineSoundControl: Rigidbody automatically assigned");
        }

        // Configure and start the engine sound
        engineSound.loop = true; // Ensure the sound loops
        engineSound.Play(); // Start playing the sound, even if volume is low
    }

    /// <summary>
    ///     Updates the engine sound pitch and volume based on vehicle speed.
    ///     Called once per frame to ensure smooth audio transitions.
    /// </summary>
    private void Update()
    {
        var speed = vehicleRigidbody.velocity.magnitude; // Get vehicle speed

        if (speed > speedThreshold)
        {
            // Adjust volume and pitch based on speed
            engineSound.volume = Mathf.Lerp(minVolume, maxVolume, speed / maxVolumeSpeed);
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, speed / maxPitchSpeed);
        }
        else
        {
            engineSound.volume = 0f; // Mute when not moving
        }
    }
}