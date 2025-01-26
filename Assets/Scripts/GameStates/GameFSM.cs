using UnityEngine;


public class GameFSM : MonoBehaviour
{
    public BaseState CurrentState { get; private set; }

    // States
    public IntroState IntroState { get; private set; }
    public GameProperState GameProperState { get; private set; }

    // Weather Manager Reference
    public WeatherManager WeatherManager;

    // Tutorial and Game Timer
    public bool IsTutorialComplete = false;

    void Start()
    {
        // Log the start for debugging purposes
        Debug.Log("GameFSM Started");
        // Initialize states
        IntroState = new IntroState(this);
        GameProperState = new GameProperState(this);

        // Start with IntroState
        TransitionToState(IntroState);
    }

    void Update()
    {
        CurrentState?.Update();
    }

    public void TransitionToState(BaseState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    // Example method to simulate tutorial completion
    public void CompleteTutorial()
    {
        IsTutorialComplete = true;
    }

    // Placeholder methods for starting tutorial and game timer
    public void StartIntroTutorial()
    {
        // Implement your tutorial logic here
        // For simplicity, we'll simulate tutorial completion after 10 seconds
        Invoke("CompleteTutorial", 10f);
    }

    public void StartGameTimer()
    {
        // Implement game timer logic here
        // Example: 10-minute shift
        Invoke("EndGame", 600f);
    }

    void EndGame()
    {
        // Handle game end logic
        Debug.Log("Game Over!");
    }
}