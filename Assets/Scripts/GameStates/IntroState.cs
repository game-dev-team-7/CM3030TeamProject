using UnityEngine;

public class IntroState : BaseState
{
    private bool isMovementTutorialComplete = false;
    private bool isCameraRotationTutorialComplete = false;
    private bool isDeliveryTutorialComplete = false;
    private bool isWeatherTutorialComplete = false;

    // Reference to the tutorial mask
    private GameObject tutorialMask1;
    private GameObject tutorialMask2;
    private GameObject tutorialMask3;
    private GameObject tutorialMask4;

    public IntroState(GameFSM fsm) : base(fsm)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Intro Stage");

        fsm.WeatherManager.SetWeather(WeatherType.Normal);

        tutorialMask1 = GameObject.Find("TutorialMask1");
        tutorialMask2 = GameObject.Find("TutorialMask2");
        tutorialMask3 = GameObject.Find("TutorialMask3");
        tutorialMask4 = GameObject.Find("TutorialMask4");

        if (tutorialMask1 != null) tutorialMask1.SetActive(false);
        if (tutorialMask2 != null) tutorialMask2.SetActive(false);
        if (tutorialMask3 != null) tutorialMask3.SetActive(false);
        if (tutorialMask4 != null) tutorialMask4.SetActive(false);
    }

    public override void Update()
    {
        base.Update();

        if (!isMovementTutorialComplete)
        {
            if (!tutorialMask1.activeSelf) tutorialMask1.SetActive(true);
            HandleMovementInput();
        }
        else if (!isCameraRotationTutorialComplete)
        {
            if (!tutorialMask2.activeSelf) tutorialMask2.SetActive(true);
            HandleRotationInput();
        }
        else if (!isDeliveryTutorialComplete)
        {
            if (!tutorialMask3.activeSelf) tutorialMask3.SetActive(true);
            HandleDeliveryInput();
        }
        else if (!isWeatherTutorialComplete)
        {
            if (!tutorialMask4.activeSelf) tutorialMask4.SetActive(true);
            HandleWeatherInput();
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

    private void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (tutorialMask1.activeSelf) tutorialMask1.SetActive(false);
            isMovementTutorialComplete = true;
        }
    }

    private void HandleRotationInput()
    {
        if (Input.GetMouseButton(1))
        {
            if (tutorialMask2.activeSelf) tutorialMask2.SetActive(false);
            isCameraRotationTutorialComplete = true;
        }
    }

    private void HandleDeliveryInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            if (tutorialMask3.activeSelf) tutorialMask3.SetActive(false);
            isDeliveryTutorialComplete = true;
        }
    }

    private void HandleWeatherInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            if (tutorialMask4.activeSelf) tutorialMask4.SetActive(false);
            isWeatherTutorialComplete = true;
        }
    }
}