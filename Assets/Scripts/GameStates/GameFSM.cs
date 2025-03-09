using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
///     Manages the game's finite state machine (FSM), controlling transitions between
///     different game states and orchestrating the game flow from intro to gameplay to game over.
/// </summary>
public class GameFSM : MonoBehaviour
{
    /// <summary>
    ///     Reference to the weather system manager.
    /// </summary>
    public WeatherManager WeatherManager;

    /// <summary>
    ///     Reference to the customer delivery system manager.
    /// </summary>
    public CustomerManager customerManager;

    // Tutorial Mask References
    /// <summary>
    ///     UI mask for the movement tutorial step.
    /// </summary>
    public GameObject tutorialMask1;

    /// <summary>
    ///     UI mask for the camera rotation tutorial step.
    /// </summary>
    public GameObject tutorialMask2;

    /// <summary>
    ///     UI mask for the delivery tutorial step.
    /// </summary>
    public GameObject tutorialMask3;

    /// <summary>
    ///     UI mask for the weather tutorial step.
    /// </summary>
    public GameObject tutorialMask4;

    private Canvas miniMapCanvas;

    private Canvas scoreCanvas;
    private Canvas temperatureBarCanvas;
    private Canvas timerCanvas;
    private Canvas weatherCanvas;

    /// <summary>
    ///     The currently active game state.
    /// </summary>
    public BaseState CurrentState { get; private set; }

    // States
    /// <summary>
    ///     The intro/tutorial state that runs at the beginning of the game.
    /// </summary>
    public IntroState IntroState { get; private set; }

    /// <summary>
    ///     The main gameplay state where core game mechanics are active.
    /// </summary>
    public GameProperState GameProperState { get; private set; }

    /// <summary>
    ///     The game over state that handles end-of-game logic.
    /// </summary>
    public GameOverState GameOverState { get; private set; }

    /// <summary>
    ///     Initializes the FSM, creates all game states, and sets up tutorial UI components.
    /// </summary>
    private void Start()
    {
        // Log the start for debugging purposes
        Debug.Log("GameFSM Started");

        // Initialize states
        IntroState = new IntroState(this);
        GameProperState = new GameProperState(this);
        GameOverState = new GameOverState(this);

        // Deactivate tutorial masks
        if (tutorialMask1 != null) tutorialMask1.SetActive(false);
        if (tutorialMask2 != null) tutorialMask2.SetActive(false);
        if (tutorialMask3 != null) tutorialMask3.SetActive(false);
        if (tutorialMask4 != null) tutorialMask4.SetActive(false);

        // Find and assign the canvases
        var scoreObject = GameObject.Find("Score");
        if (scoreObject != null)
            scoreCanvas = scoreObject.GetComponent<Canvas>();
        else
            Debug.LogError("Score GameObject not found!");

        var timerObject = GameObject.Find("CustomerTimer");
        if (timerObject != null)
            timerCanvas = timerObject.GetComponent<Canvas>();
        else
            Debug.LogError("Timer GameObject not found!");

        var temperatureBarObject = GameObject.Find("TemperatureBar");
        if (temperatureBarObject != null)
            temperatureBarCanvas = temperatureBarObject.GetComponent<Canvas>();
        else
            Debug.LogError("TemperatureBar GameObject not found!");

        var weatherObject = GameObject.Find("WeatherChangeNotifier");
        if (weatherObject != null)
            weatherCanvas = weatherObject.GetComponent<Canvas>();
        else
            Debug.LogError("Weather GameObject not found!");

        var miniMapObject = GameObject.Find("MiniMapCanvas");
        if (miniMapObject != null)
            miniMapCanvas = miniMapObject.GetComponent<Canvas>();
        else
            Debug.LogError("MiniMap GameObject not found!");

        // Start with IntroState
        TransitionToState(IntroState);
    }

    /// <summary>
    ///     Updates the current state each frame.
    /// </summary>
    private void Update()
    {
        CurrentState?.Update();
    }

    /// <summary>
    ///     Transitions the game from the current state to a new state.
    ///     Handles proper exit and entry logic for both states.
    /// </summary>
    /// <param name="newState">The state to transition to</param>
    public void TransitionToState(BaseState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        if (CurrentState is GameProperState)
            StartCoroutine(CountdownToGameProperState());
        else
            CurrentState.Enter();
    }

    /// <summary>
    ///     Handles the countdown transition into the main gameplay state.
    ///     Displays a visual countdown from 3 to prepare the player.
    /// </summary>
    /// <returns>IEnumerator for the coroutine</returns>
    private IEnumerator CountdownToGameProperState()
    {
        var countdownText = GameObject.FindGameObjectWithTag("TutorialCountdown").GetComponent<TextMeshProUGUI>();

        countdownText.text = "Ready"; // Start with 'ready'
        yield return new WaitForSeconds(1);

        for (var i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "Start"; // Show 'start' after countdown
        yield return new WaitForSeconds(1);

        countdownText.text = "";
        GameProperState.Enter();
    }

    /// <summary>
    ///     Handles end-of-game logic.
    /// </summary>
    private void EndGame()
    {
        // Handle game end logic
        Debug.Log("Game Over!");
    }

    /// <summary>
    ///     Initiates the tutorial sequence for first-time players.
    /// </summary>
    public void StartTutorialSequence()
    {
        StartCoroutine(TutorialCoroutine());
    }

    /// <summary>
    ///     Manages the sequential tutorial steps, waiting for player input at each stage.
    /// </summary>
    /// <returns>IEnumerator for the coroutine</returns>
    private IEnumerator TutorialCoroutine()
    {
        // Show Movement Tutorial
        tutorialMask1.SetActive(true);
        yield return new WaitUntil(() =>
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
        yield return new WaitForSeconds(0.5f);
        tutorialMask1.SetActive(false);

        // Show Camera Rotation Tutorial
        tutorialMask2.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButton(1));
        yield return new WaitForSeconds(1f);
        tutorialMask2.SetActive(false);

        // Show Delivery Tutorial
        tutorialMask3.SetActive(true);
        scoreCanvas.sortingOrder = 100;
        timerCanvas.sortingOrder = 100;
        miniMapCanvas.sortingOrder = 100;
        yield return new WaitUntil(() => Input.GetKey(KeyCode.Return));
        yield return new WaitForSeconds(0.5f);
        tutorialMask3.SetActive(false);
        scoreCanvas.sortingOrder = 0;
        timerCanvas.sortingOrder = 0;


        // Show Weather Tutorial
        tutorialMask4.SetActive(true);
        temperatureBarCanvas.sortingOrder = 100;
        weatherCanvas.sortingOrder = 100;
        yield return new WaitUntil(() => Input.GetKey(KeyCode.Return));
        yield return new WaitForSeconds(0.5f);
        tutorialMask4.SetActive(false);
        temperatureBarCanvas.sortingOrder = 0;
        weatherCanvas.sortingOrder = 0;
        miniMapCanvas.sortingOrder = 0;

        // Transition to Game Proper State
        TransitionToState(GameProperState);
    }
}