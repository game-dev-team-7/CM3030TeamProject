using UnityEngine;

public class IntroState : BaseState
{
    // PlayerPrefs key to track first launch
    private const string FIRST_LAUNCH_KEY = "IsFirstLaunch";

    public IntroState(GameFSM fsm) : base(fsm)
    {
    }

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

    // Check if this is the first launch
    private bool IsFirstLaunch()
    {
        // By default, return true (first launch) if the key doesn't exist
        return PlayerPrefs.GetInt(FIRST_LAUNCH_KEY, 1) == 1;
    }

    // Mark the first launch as complete
    private void MarkFirstLaunchComplete()
    {
        PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 0);
        PlayerPrefs.Save(); // Ensure the change is saved immediately
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        Debug.Log("Exiting Intro Stage");
    }
}