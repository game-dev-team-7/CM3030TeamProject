using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float rotationSpeed = 10f;
    public float decelerationRate = 3f;

    [Header("Audio Settings")] public AudioSource vehicleAudioSource;

    public AudioSource vehicleStopAudioSource;
    private Vector3 currentDirection;
    private Camera mainCamera;

    private Rigidbody rb;

    private bool wasMoving;

    /// <summary>
    ///     Initializes the player controller.
    ///     Retrieves the rigidbody and main camera components. Also sets the rigidbody's
    ///     freezeRotation property to true so that the player's rotation is not changed
    ///     by physics.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent rigidbody from changing rotation
        mainCamera = Camera.main;
    }


    /// <summary>
    ///     Called once per frame. Updates the game logic and the player's movement.
    /// </summary>
    private void Update()
    {
        HandleInput();
        HandleEngineSound();
    }


    /// <summary>
    ///     Called every fixed framerate frame, should be used for physics related updates.
    ///     Handles player movement and rotation.
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }


    /// <summary>
    ///     Handles input for the player.
    ///     Listens for horizontal and vertical input, and uses it to calculate the
    ///     direction the player should move in. The direction is calculated by
    ///     taking the forward and right vectors of the main camera, and multiplying
    ///     them by the vertical and horizontal inputs, respectively. The resulting
    ///     vector is then normalized.
    /// </summary>
    private void HandleInput()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        // Get the camera's forward and right vectors
        var cameraForward = mainCamera.transform.forward;
        var cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        currentDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
    }


    /// <summary>
    ///     Handles the movement of the player.
    ///     Calculates the target velocity of the player based on the current direction
    ///     and the move speed. If the current direction is zero (i.e. no input), the
    ///     target velocity will be the current velocity, but with a deceleration effect
    ///     applied. Then, the target velocity is applied to the rigidbody, preserving
    ///     the Y velocity for gravity.
    /// </summary>
    private void HandleMovement()
    {
        if (currentDirection != Vector3.zero)
        {
            // Full speed immediately if there's input
            rb.velocity = currentDirection * moveSpeed;
        }
        else
        {
            // Gradual slowdown for each axis
            var newX = Mathf.MoveTowards(rb.velocity.x, 0, decelerationRate * Time.fixedDeltaTime);
            var newZ = Mathf.MoveTowards(rb.velocity.z, 0, decelerationRate * Time.fixedDeltaTime);
            rb.velocity = new Vector3(newX, rb.velocity.y, newZ);
        }
    }


    /// <summary>
    ///     Handles the rotation of the player.
    ///     If the current direction is not zero (i.e. there is input), the player's
    ///     rotation is smoothly changed to face the target direction using
    ///     Quaternion.LookRotation and Quaternion.Slerp.
    /// </summary>
    private void HandleRotation()
    {
        if (currentDirection != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(currentDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    /// <summary>
    ///     Checks if the player is moving. Plays the looping engine sound when moving,
    ///     stops it when not, and plays a one-time 'stop' sound if we've just come to a stop.
    /// </summary>
    private void HandleEngineSound()
    {
        // Already have velocity-based speed
        var horizontalSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

        // Check if there's input 
        var hasInput = Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f;

        // If you need velocity for partial movement, you could combine them:
        // e.g. treat it as “moving” if there's input OR speed > some threshold:
        var isMoving = hasInput || horizontalSpeed > 0.001f;

        if (isMoving)
        {
            if (!vehicleAudioSource.isPlaying)
                vehicleAudioSource.Play();
        }
        else
        {
            if (vehicleAudioSource.isPlaying)
                vehicleAudioSource.Stop();

            if (wasMoving && vehicleStopAudioSource != null) vehicleStopAudioSource.Play();
        }

        wasMoving = isMoving;
    }
}