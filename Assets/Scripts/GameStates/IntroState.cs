using UnityEngine;


public class IntroState : BaseState
{
    public IntroState(GameFSM fsm) : base(fsm)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Intro Stage");
        fsm.WeatherManager.SetWeather(WeatherType.Mild);
        fsm.StartIntroTutorial();
    }

    public override void Update()
    {
        base.Update();
        // Check if tutorial is complete to transition to GameProper
        if (fsm.IsTutorialComplete)
        {
            fsm.TransitionToState(fsm.GameProperState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Intro Stage");
    }
}