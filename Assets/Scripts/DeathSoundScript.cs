using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField]private AudioSource deathSound;

    private void GameOver()
    {
        // Play death sound
        if (deathSound != null)
        {
            deathSound.Play();
        }
        gameObject.SetActive(false); //Deactivates the player
    }
}
