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

    private void Start()
    {
        // Log the start for debugging purposes
        Debug.Log("GameFSM Started");

        // Initialize states
        IntroState = new IntroState(this);
        GameProperState = new GameProperState(this);
        GameOverState = new GameOverState(this);

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
}