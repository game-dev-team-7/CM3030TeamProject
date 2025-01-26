using UnityEngine;

public class GameProperState : BaseState
{
    public GameProperState(GameFSM fsm) : base(fsm)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Game Proper Stage");
        fsm.WeatherManager.StartWeatherCycle();
        fsm.StartGameTimer();
    }

    public override void Update()
    {
        base.Update();
        // Handle game logic updates here
    }

    public override void Exit()
    {
        Debug.Log("Exiting Game Proper Stage");
        fsm.WeatherManager.StopWeatherCycle();
    }
}