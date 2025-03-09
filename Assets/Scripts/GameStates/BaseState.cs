/// <summary>
///     Base class for all game states in the finite state machine (FSM) pattern.
///     Provides core functionality and structure for derived state classes.
/// </summary>
public class BaseState
{
    /// <summary>
    ///     Reference to the game's finite state machine manager.
    ///     Protected access allows derived states to interact with the FSM.
    /// </summary>
    protected GameFSM fsm;

    /// <summary>
    ///     Constructor that initializes the state with a reference to the FSM.
    /// </summary>
    /// <param name="fsm">Reference to the game's finite state machine</param>
    public BaseState(GameFSM fsm)
    {
        this.fsm = fsm;
    }

    /// <summary>
    ///     Called when entering this state.
    ///     Override in derived classes to implement state-specific initialization.
    /// </summary>
    public virtual void Enter()
    {
    }

    /// <summary>
    ///     Called when exiting this state.
    ///     Override in derived classes to implement state-specific cleanup.
    /// </summary>
    public virtual void Exit()
    {
    }

    /// <summary>
    ///     Called every frame while this state is active.
    ///     Override in derived classes to implement state-specific behavior.
    /// </summary>
    public virtual void Update()
    {
    }
}