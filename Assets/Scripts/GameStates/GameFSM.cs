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

        countdownText.text = ""; // Start with empty text
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
        yield return new WaitForSeconds(0.5f);
        tutorialMask2.SetActive(false);

        // Show Delivery Tutorial
        tutorialMask3.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.anyKeyDown);
        yield return new WaitForSeconds(0.5f);
        tutorialMask3.SetActive(false);

        // Show Weather Tutorial
        tutorialMask4.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.anyKeyDown);
        yield return new WaitForSeconds(0.5f);
        tutorialMask4.SetActive(false);

        // Transition to Game Proper State
        TransitionToState(GameProperState);
    }
}