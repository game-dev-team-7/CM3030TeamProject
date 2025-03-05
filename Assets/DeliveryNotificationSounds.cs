using UnityEngine;

public class DeliveryNotificationSounds : MonoBehaviour
{
    [Header("Task Sounds")]
    
    public AudioClip successSound;
    public AudioClip failureSound;

    private AudioSource audioSource;

    void Start()
    {
        //Gets the AudioSource component on the player
        audioSource = GetComponent<AudioSource>();
    }

    //Plays success sound
    public void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }

    //Plays failure sound
    public void PlayFailureSound()
    {
        if (audioSource != null && failureSound != null)
        {
            audioSource.PlayOneShot(failureSound);
        }
    }
}
