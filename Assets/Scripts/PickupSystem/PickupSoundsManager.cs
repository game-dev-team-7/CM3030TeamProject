using UnityEngine;

public class PlayerPickupSounds : MonoBehaviour
{
    [Header("Pickup Sounds")]
    public AudioClip winterCoatSound;
    public AudioClip tShirtSound;
    public AudioClip lemonadeSound;
    public AudioClip hotChocolateSound;

    private AudioSource audioSource;

    void Start()
    {
        //Gets the AudioSource component on the player
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPickupSound(string itemType)
    {
        //Based on the item picked up, this plays the appropriate sound
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
        }
    }
}
