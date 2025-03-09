using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
///     Manages the pause functionality during gameplay.
///     Handles time scale adjustments and pause menu UI display.
/// </summary>
public class PauseManager : MonoBehaviour
{
    /// <summary>
    ///     Reference to the pause menu UI panel.
    /// </summary>
    public GameObject pauseMenuUI;

    /// <summary>
    ///     Tracks whether the game is currently paused.
    /// </summary>
    private bool isPaused;

    /// <summary>
    ///     Checks for pause key input and toggles the pause state.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    /// <summary>
    ///     Toggles the game between paused and unpaused states.
    ///     Updates UI visibility and time scale accordingly.
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenuUI.SetActive(isPaused);
    }

    /// <summary>
    ///     Continues the game from the paused state.
    /// </summary>
    public void ContinueGame()
    {
        TogglePause();

        // Deselect any selected UI button
        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    ///     Restarts the current game scene.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time is normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}