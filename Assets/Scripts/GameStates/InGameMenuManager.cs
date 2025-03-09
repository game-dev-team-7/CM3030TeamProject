using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
///     Manages in-game menus including pause functionality and game over screens.
///     Handles UI transitions, game state pausing, and scene management.
/// </summary>
public class InGameMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    /// <summary>
    /// Reference to the pause menu UI panel.
    /// </summary>
    public GameObject pauseMenuUI;

    /// <summary>
    ///     Reference to the game over UI panel.
    /// </summary>
    public GameObject gameOverPanel;

    [Header("Game Over Text")]
    /// <summary>
    /// Main text displayed on the game over screen.
    /// </summary>
    public TextMeshProUGUI gameOverText;

    /// <summary>
    ///     Text displaying temperature-related information on game over.
    /// </summary>
    public TextMeshProUGUI temperatureInfoText;

    /// <summary>
    ///     Text displaying weather-related information on game over.
    /// </summary>
    public TextMeshProUGUI weatherInfoText;

    [Header("Weather References")]
    /// <summary>
    /// Reference to the weather manager for accessing weather state.
    /// </summary>
    public WeatherManager weatherManager;

    /// <summary>
    ///     Reference to the player temperature system for accessing temperature state.
    /// </summary>
    public PlayerTemperature playerTemperature;

    [Header("Score Text")]
    /// <summary>
    /// Reference to the score text for accessing final score on game over.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI scoreText;

    /// <summary>
    ///     Prevents the game over screen from being displayed multiple times.
    /// </summary>
    private bool hasGameOverBeenShown;

    /// <summary>
    ///     Tracks whether the game is currently paused.
    /// </summary>
    private bool isPaused;

    /// <summary>
    ///     Tracks whether the game audio has been stopped.
    /// </summary>
    private bool isStopped;

    /// <summary>
    ///     Initializes component references if not set in the inspector.
    /// </summary>
    private void Start()
    {
        // Find references if not assigned in inspector
        if (weatherManager == null) weatherManager = FindObjectOfType<WeatherManager>();

        if (playerTemperature == null) playerTemperature = FindObjectOfType<PlayerTemperature>();
    }

    /// <summary>
    ///     Checks for pause key input and toggles the pause state.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !hasGameOverBeenShown) TogglePause();
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

        // Deselect selected button to avoid issues
        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    ///     Continues the game from the paused state.
    /// </summary>
    public void ContinueGame()
    {
        TogglePause();
    }

    /// <summary>
    ///     Displays the game over screen with detailed information about the player's demise.
    ///     Called when game-ending conditions are met.
    /// </summary>
    public void ShowGameOver()
    {
        if (hasGameOverBeenShown) return; // Prevent multiple activations
        hasGameOverBeenShown = true;

        Debug.Log("Game Over");

        // Setup game over text
        UpdateGameOverInfo();

        // Show game over panel
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        //Mutes all sounds except UI
        ToggleGameSounds();
    }

    /// <summary>
    ///     Updates the game over screen with specific information about how the player died.
    /// </summary>
    private void UpdateGameOverInfo()
    {
        // Default message if we can't determine the cause
        var mainMessage = "Game Over!";

        // Get temperature-specific info if available
        if (playerTemperature != null)
        {
            var bodyTemp = playerTemperature.bodyTemperature;

            if (bodyTemp <= playerTemperature.minTemperature)
                mainMessage = "You froze to death!";
            else if (bodyTemp >= playerTemperature.maxTemperature) mainMessage = "You died from heatstroke!";

            //Updates main game over text
            if (gameOverText != null) gameOverText.text = mainMessage;
        }
    }

    /// <summary>
    ///     Restarts the current game scene.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; //Resets time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    ///     Returns to the main menu and saves high score.
    /// </summary>
    public void ReturnToMainMenu()
    {
        //Save Score as High Score (unless it is less than current High Score)
        FindObjectOfType<HighScoreManager>().SaveHighScore(int.Parse(scoreText.text));

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); //Returns to main menu
    }

    /// <summary>
    ///     Toggles game sounds on and off, except for UI sounds.
    /// </summary>
    public void ToggleGameSounds()
    {
        isStopped = !isStopped;
        //Finds all audio sources in the scene
        var allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (var audioSource in allAudioSources)
        {
            //Checks to disregard sounds with the appropriate tag.
            if (audioSource.gameObject.CompareTag("UISounds")) continue;

            //Stops and mutes all other sounds.
            audioSource.Stop();
        }
    }
}