using UnityEngine;

/// <summary>
///     Manages the audio feedback for different pickup items in the game.
///     Plays appropriate sounds when the player interacts with various collectible items.
/// </summary>
public class PlayerPickupSounds : MonoBehaviour
{
    [Header("Pickup Sounds")]
    /// <summary>Sound played when picking up a winter coat</summary>
    public AudioClip winterCoatSound;

    /// <summary>Sound played when picking up a t-shirt</summary>
    public AudioClip tShirtSound;

    /// <summary>Sound played when picking up lemonade</summary>
    public AudioClip lemonadeSound;

    /// <summary>Sound played when picking up hot chocolate</summary>
    public AudioClip hotChocolateSound;

    /// <summary>Audio source component used to play the sounds</summary>
    private AudioSource audioSource;

    /// <summary>
    ///     Initializes the audio source component reference.
    /// </summary>
    private void Start()
    {
        // Gets the AudioSource component on the player
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            Debug.LogError("[PlayerPickupSounds] No AudioSource component found on this GameObject.");
    }

    /// <summary>
    ///     Plays the appropriate sound based on the type of item picked up.
    /// </summary>
    /// <param name="itemType">The type of item picked up (e.g., "WinterCoat", "TShirt")</param>
    public void PlayPickupSound(string itemType)
    {
        // Guard clause to prevent null reference exceptions
        if (audioSource == null) return;

        // Based on the item picked up, this plays the appropriate sound
        switch (itemType)
        {
            case "WinterCoat":
                if (winterCoatSound != null) audioSource.PlayOneShot(winterCoatSound);
                break;
            case "TShirt":
                if (tShirtSound != null) audioSource.PlayOneShot(tShirtSound);
                break;
            case "Lemonade":
                if (lemonadeSound != null) audioSource.PlayOneShot(lemonadeSound);
                break;
            case "HotChocolate":
                if (hotChocolateSound != null) audioSource.PlayOneShot(hotChocolateSound);
                break;
            default:
                Debug.LogWarning($"[PlayerPickupSounds] No sound defined for item type: {itemType}");
                break;
        }
    }
}