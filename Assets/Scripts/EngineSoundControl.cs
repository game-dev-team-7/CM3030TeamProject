using UnityEngine;

public class EngineSoundControl : MonoBehaviour
{
    public AudioSource engineSound; // Reference to the vehicle's audio source
    public Rigidbody vehicleRigidbody; // Reference to the vehicle's Rigidbody
    public float minPitch = 0.5f; // Pitch at idle
    public float maxPitch = 2.0f; // Pitch at max speed
    public float speedThreshold = 0.5f; // Minimum speed to play sound

    private void Start()
    {
        if (engineSound == null)
        {
            engineSound = GetComponent<AudioSource>(); // Get AudioSource if not assigned
        }

        if (vehicleRigidbody == null)
        {
            vehicleRigidbody = GetComponent<Rigidbody>(); // Get Rigidbody if not assigned
        }

        engineSound.loop = true; // Ensure the sound loops
        engineSound.Play(); // Start playing the sound, even if volume is low
    }

    private void Update()
    {
        float speed = vehicleRigidbody.velocity.magnitude; // Get vehicle speed

        if (speed > speedThreshold)
        {
            // Adjust volume and pitch based on speed
            engineSound.volume = Mathf.Lerp(0.2f, 0.5f, speed / 20f);
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, speed / 50f);
        }
        else
        {
            engineSound.volume = 0f; // Mute when not moving
        }
    }
}
