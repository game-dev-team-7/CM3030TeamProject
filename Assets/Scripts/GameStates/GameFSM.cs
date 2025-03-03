using UnityEngine;


public class GameFSM : MonoBehaviour
{
    public BaseState CurrentState { get; private set; }

    // States
    public IntroState IntroState { get; private set; }
    public GameProperState GameProperState { get; private set; }
    public GameOverState GameOverState { get; private set; }  // NEW


    // Weather Manager Reference
    public WeatherManager WeatherManager;

    void Start()
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