using UnityEngine;

/// <summary>
///     Manages the introduction and tutorial state at the beginning of the game.
///     Handles tutorial display logic and first-time player experience.
/// </summary>
public class IntroState : BaseState
{
    /// <summary>
    ///     PlayerPrefs key used to track whether this is the player's first time playing.
    /// </summary>
    private const string FIRST_LAUNCH_KEY = "IsFirstLaunch";

    /// <summary>
    ///     Initializes a new instance of the IntroState.
    /// </summary>
    /// <param name="fsm">Reference to the game's finite state machine</param>
    public IntroState(GameFSM fsm) : base(fsm)
    {
    }

    /// <summary>
    ///     Called when entering the intro state.
    ///     Determines whether to show the tutorial or skip to gameplay based on player history.
    /// </summary>
    public override void Enter()
    {
        Debug.Log("Entering Intro Stage");
        fsm.WeatherManager.StopWeatherCycle();
        fsm.customerManager.DisableManager();

        // Check if this is the first launch
        if (IsFirstLaunch())
        {
            // Start tutorial and mark as not first launch anymore
            MarkFirstLaunchComplete();
            fsm.StartTutorialSequence();
        }
        else
        {
            // Skip tutorial, go directly to game proper state
            fsm.TransitionToState(fsm.GameProperState);
        }
    }

    /// <summary>
    ///     Checks if this is the player's first time launching the game.
    /// </summary>
    /// <returns>True if this is the first launch, false otherwise</returns>
    private bool IsFirstLaunch()
    {
        // By default, return true (first launch) if the key doesn't exist
        return PlayerPrefs.GetInt(FIRST_LAUNCH_KEY, 1) == 1;
    }

    /// <summary>
    ///     Marks the first launch as complete in PlayerPrefs.
    /// </summary>
    private void MarkFirstLaunchComplete()
    {
        PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 0);
        PlayerPrefs.Save(); // Ensure the change is saved immediately
    }

    /// <summary>
    ///     Called when exiting the intro state.
    /// </summary>
    public override void Exit()
    {
        Debug.Log("Exiting Intro Stage");
    }
}