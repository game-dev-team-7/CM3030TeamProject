using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class InGameMenuManager : MonoBehaviour
{
    [Header("UI Panels")] public GameObject pauseMenuUI;
    public GameObject gameOverPanel;

    [Header("Game Over Text")] public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI temperatureInfoText;
    public TextMeshProUGUI weatherInfoText;

    [Header("Weather References")] public WeatherManager weatherManager;
    public PlayerTemperature playerTemperature;

    [Header("Score Text")]
    [SerializeField] private TextMeshProUGUI scoreText; 

    private bool isPaused = false;
    private bool hasGameOverBeenShown = false;

    private bool isStopped = false;//Tracks whether the game audio has been stopped, and initialises it as false.

    private void Start()
    {
        // Find references if not assigned in inspector
        if (weatherManager == null)
        {
            weatherManager = FindObjectOfType<WeatherManager>();
        }

        if (playerTemperature == null)
        {
            playerTemperature = FindObjectOfType<PlayerTemperature>();
        }
    }

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

    // Show Game Over Screen with detailed information
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

    // Update game over information with detailed temperature and weather data
    private void UpdateGameOverInfo()
    {
        // Default message if we can't determine the cause
        string mainMessage = "Game Over!";

        // Get temperature-specific info if available
        if (playerTemperature != null)
        {
            float bodyTemp = playerTemperature.bodyTemperature;

            if (bodyTemp <= playerTemperature.minTemperature)
            {
                mainMessage = "You froze to death!";
            }
            else if (bodyTemp >= playerTemperature.maxTemperature)
            {
                mainMessage = "You died from heatstroke!";
            }

            //Updates main game over text
            if (gameOverText != null)
            {
                gameOverText.text = mainMessage;
            }
        }
    }


    // Restart the game
    public void RestartGame()
    {
        Time.timeScale = 1f;//Resets time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Return to Main Menu
    public void ReturnToMainMenu()
    {
        //Save Score as High Score (unless it is less than current High Score)
        FindObjectOfType<HighScoreManager>().SaveHighScore(int.Parse(scoreText.text));

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");//Returns to main menu
    }

    public void ToggleGameSounds()
    {
        isStopped = !isStopped;
        //Finds all audio sources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in allAudioSources)
        {
            //Checks to disregard sounds with the appropriate tag.
            if (audioSource.gameObject.CompareTag("UISounds"))
            {
                continue;
            }

            //Stops and mutes all other sounds.
            audioSource.Stop();
        }
    }
}
