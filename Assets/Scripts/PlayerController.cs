using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float rotationSpeed = 10f;
    public float decelerationRate = 15f;

    private Rigidbody rb;
    private Vector3 currentDirection;
    private Camera mainCamera;

    /// <summary>
    /// Initializes the player controller.
    /// 
    /// Retrieves the rigidbody and main camera components. Also sets the rigidbody's
    /// freezeRotation property to true so that the player's rotation is not changed
    /// by physics.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent rigidbody from changing rotation
        mainCamera = Camera.main;
    }


    /// <summary>
    /// Called once per frame. Updates the game logic and the player's movement.
    /// </summary>
    private void Update()
    {
        HandleInput();
    }


    /// <summary>
    /// Called every fixed framerate frame, should be used for physics related updates.
    /// 
    /// Handles player movement and rotation.
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }


    /// <summary>
    /// Handles input for the player.
    /// 
    /// Listens for horizontal and vertical input, and uses it to calculate the
    /// direction the player should move in. The direction is calculated by
    /// taking the forward and right vectors of the main camera, and multiplying
    /// them by the vertical and horizontal inputs, respectively. The resulting
    /// vector is then normalized.
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
    /// Handles the movement of the player.
    /// 
    /// Calculates the target velocity of the player based on the current direction
    /// and the move speed. If the current direction is zero (i.e. no input), the
    /// target velocity will be the current velocity, but with a deceleration effect
    /// applied. Then, the target velocity is applied to the rigidbody, preserving
    /// the Y velocity for gravity.
    /// </summary>
    private void HandleMovement()
    {
        Vector3 targetVelocity;

        if (currentDirection != Vector3.zero)
            // Calculate the desired velocity when there's input
            targetVelocity = currentDirection * moveSpeed;
        else
            // When no input, gradually reduce velocity
            targetVelocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelerationRate * Time.fixedDeltaTime);

        // Apply the velocity to the X and Z axes, preserving Y velocity for gravity
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }


    /// <summary>
    /// Handles the rotation of the player.
    /// 
    /// If the current direction is not zero (i.e. there is input), the player's
    /// rotation is smoothly changed to face the target direction using
    /// Quaternion.LookRotation and Quaternion.Slerp.
    /// </summary>
    private void HandleRotation()
    {
        if (currentDirection != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(currentDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}