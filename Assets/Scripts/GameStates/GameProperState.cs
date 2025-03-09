using UnityEngine;

/// <summary>
///     Manages the main gameplay state where core game mechanics are active.
///     Controls the activation of weather cycles and customer delivery systems.
/// </summary>
public class GameProperState : BaseState
{
    /// <summary>
    ///     Initializes a new instance of the GameProperState.
    /// </summary>
    /// <param name="fsm">Reference to the game's finite state machine</param>
    public GameProperState(GameFSM fsm) : base(fsm)
    {
    }

    /// <summary>
    ///     Called when entering the main gameplay state.
    ///     Activates core game systems including weather and customer deliveries.
    /// </summary>
    public override void Enter()
    {
        Debug.Log("Entering Game Proper Stage");
        fsm.WeatherManager.StartWeatherCycle();
        fsm.customerManager.EnableManager();
    }

    /// <summary>
    ///     Called when exiting the main gameplay state.
    ///     Deactivates core game systems to prevent them from running in other states.
    /// </summary>
    public override void Exit()
    {
        Debug.Log("Exiting Game Proper Stage");
        fsm.WeatherManager.StopWeatherCycle();
        fsm.customerManager.DisableManager();
    }
}