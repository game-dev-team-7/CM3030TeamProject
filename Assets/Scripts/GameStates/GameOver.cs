using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    private bool hasGameOverBeenShown = false; // Prevent multiple triggers

    public void ShowGameOver()
    {
        // Only trigger once
        if (hasGameOverBeenShown) return;
        hasGameOverBeenShown = true;

        Debug.Log("Show Game Over");
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;  // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Ensure this matches your main menu scene name
    }
}