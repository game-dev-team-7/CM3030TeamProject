using UnityEngine;

public class IntroState : BaseState
{
    public IntroState(GameFSM fsm) : base(fsm)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Intro Stage");

        fsm.WeatherManager.SetWeather(WeatherType.Normal);

        fsm.StartTutorialSequence();
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