using UnityEngine;


public class IntroState : BaseState
{
    private bool isMovementTutorialComplete = false;

    private bool isCameraRotationTutorialComplete = false;

    private bool isDeliveryTutorialComplete = false;

    private bool isWeatherTutorialComplete = false;

    public IntroState(GameFSM fsm) : base(fsm)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Intro Stage");
        fsm.WeatherManager.SetWeather(WeatherType.Normal);
    }

    public override void Update()
    {
        base.Update();

        if (!isMovementTutorialComplete)
        {
            ShowMovementTutorial();
            HandleMovementInput();
        }
        else if (!isCameraRotationTutorialComplete)
        {
            showRotationTutorial();
            HandleRotationInput();
        }
        else if (!isDeliveryTutorialComplete)
        {
            showDeliveryTutorial();
            handleDeliveryInput();
        }
        else if (!isWeatherTutorialComplete)
        {
            showWeatherTutorial();
            handleWeatherInput();
        }
        else
        {
            fsm.TransitionToState(fsm.GameProperState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Intro Stage");
    }

    private void ShowMovementTutorial()
    {
        Debug.Log("Use W, A, S, and D keys to move your character in any direction");
    }


    private void HandleMovementInput()
    {
        // Check for WSAD input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            Debug.Log("moved");
            isMovementTutorialComplete = true;
        }
    }

    private void showRotationTutorial()
    {
        Debug.Log("Hold down the Right Mouse Button to look around and control the camera");
    }

    private void HandleRotationInput()
    {
        // Check for mouse input
        if (Input.GetMouseButton(1))
        {
            Debug.Log("rotated");
            isCameraRotationTutorialComplete = true;
        }
    }
    
    private void showDeliveryTutorial()
    {
        Debug.Log("Complete tasks to earn the highest score");
    }
    
    private void handleDeliveryInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            isDeliveryTutorialComplete = true;
        }
    }
    
    private void showWeatherTutorial()
    {
        Debug.Log("Pick up supplies to survive weather changes");
    }
    
    private void handleWeatherInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            isWeatherTutorialComplete = true;
        }
    }
}