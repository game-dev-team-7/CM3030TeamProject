using UnityEngine;

/// <summary>
///     Manages audio feedback for delivery task completion status.
///     Plays appropriate sounds for success and failure states.
/// </summary>
public class DeliveryNotificationSounds : MonoBehaviour
{
    [Header("Task Sounds")] [Tooltip("Sound played when a delivery is successful")]
    public AudioClip successSound;

    [Tooltip("Sound played when a delivery fails")]
    public AudioClip failureSound;

    private AudioSource audioSource;

    /// <summary>
    ///     Initializes the notification sound system by getting the AudioSource component.
    /// </summary>
    private void Start()
    {
        // Gets the AudioSource component on the player
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("DeliveryNotificationSounds: No AudioSource component found!");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    ///     Plays the success sound when a delivery is completed successfully.
    /// </summary>
    public void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
            Debug.Log("Delivery success sound played");
        }
        else
        {
            Debug.LogWarning("Cannot play success sound: AudioSource or success clip is missing");
        }
    }

    /// <summary>
    ///     Plays the failure sound when a delivery fails.
    /// </summary>
    public void PlayFailureSound()
    {
        if (audioSource != null && failureSound != null)
        {
            audioSource.PlayOneShot(failureSound);
            Debug.Log("Delivery failure sound played");
        }
        else
        {
            Debug.LogWarning("Cannot play failure sound: AudioSource or failure clip is missing");
        }
    }
}