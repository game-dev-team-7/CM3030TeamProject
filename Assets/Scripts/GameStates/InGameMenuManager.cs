using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class InGameMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject gameOverPanel;

    private bool isPaused = false;
    private bool hasGameOverBeenShown = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !hasGameOverBeenShown)
        {
            TogglePause();
        }
    }

    // Toggle Pause Menu
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenuUI.SetActive(isPaused);

        // Deselect selected button to avoid issues
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Continue Game from Pause Menu
    public void ContinueGame()
    {
        TogglePause();
    }

    // Show Game Over Screen
    public void ShowGameOver()
    {
        if (hasGameOverBeenShown) return; // Prevent multiple activations
        hasGameOverBeenShown = true;

        Debug.Log("Game Over");
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;  // Pause the game
    }

    // Restart the game
    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Return to Main Menu
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Change to match your main menu scene name
    }
}
