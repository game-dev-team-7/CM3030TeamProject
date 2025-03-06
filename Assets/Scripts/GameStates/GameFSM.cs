using System;
using UnityEngine;
using TMPro;
using System.Collections;

public class GameFSM : MonoBehaviour
{
    public BaseState CurrentState { get; private set; }

    // States
    public IntroState IntroState { get; private set; }
    public GameProperState GameProperState { get; private set; }
    public GameOverState GameOverState { get; private set; }

    // Weather Manager Reference
    public WeatherManager WeatherManager;
    // Reference to CustomerManager
    public CustomerManager customerManager;

    // Tutorial Mask References
    public GameObject tutorialMask1;
    public GameObject tutorialMask2;
    public GameObject tutorialMask3;
    public GameObject tutorialMask4;

    private Canvas scoreCanvas;
    private Canvas timerCanvas;
    private Canvas temperatureBarCanvas;
    private Canvas weatherCanvas;
    private Canvas miniMapCanvas;

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
        GameObject scoreObject = GameObject.Find("Score");
        if (scoreObject != null)
        {
            scoreCanvas = scoreObject.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("Score GameObject not found!");
        }
        
        GameObject timerObject = GameObject.Find("CustomerTimer");
        if (timerObject != null)
        {
            timerCanvas = timerObject.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("Timer GameObject not found!");
        }

        GameObject temperatureBarObject = GameObject.Find("TemperatureBar");
        if (temperatureBarObject != null)
        {
            temperatureBarCanvas = temperatureBarObject.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("TemperatureBar GameObject not found!");
        }
        
        GameObject weatherObject = GameObject.Find("WeatherChangeNotifier");
        if (weatherObject != null)
        {
            weatherCanvas = weatherObject.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("Weather GameObject not found!");
        }

        GameObject miniMapObject = GameObject.Find("MiniMapCanvas");
        if (miniMapObject != null)
        {
            miniMapCanvas = miniMapObject.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("MiniMap GameObject not found!");
        }
        
        // Start with IntroState
        TransitionToState(IntroState);
    }

    private void Update()
    {
        CurrentState?.Update();
    }


    public void TransitionToState(BaseState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        if (CurrentState is GameProperState)
            StartCoroutine(CountdownToGameProperState());
        else
            CurrentState.Enter();
    }

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

    private void EndGame()
    {
        // Handle game end logic
        Debug.Log("Game Over!");
    }

    public void StartTutorialSequence()
    {
        StartCoroutine(TutorialCoroutine());
    }

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