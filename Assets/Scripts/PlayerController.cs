using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float rotationSpeed = 10f;
    public float decelerationRate = 15f;

    private Rigidbody rb;
    private Vector3 currentDirection;


    /// <summary>
    /// Called once at the start of the game.
    /// Grabs a reference to the attached Rigidbody and sets its rotation to be frozen.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent rigidbody from changing rotation
    }

    /// <summary>
    /// Called once per frame.
    /// Checks for input and sets <see cref="currentDirection"/> accordingly.
    /// </summary>
    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// Handles player movement and rotation.
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    /// <summary>
    /// Called every frame.
    /// Checks for input and sets <see cref="currentDirection"/> accordingly.
    /// <see cref="currentDirection"/> is set to the normalized result of <see cref="Input.GetAxis(string)"/>
    /// with "Horizontal" and "Vertical" as the arguments.
    /// </summary>
    private void HandleInput()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        currentDirection = new Vector3(horizontal, 0, vertical).normalized;
    }

    /// <summary>
    /// Called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// Handles player movement.
    /// When there is input, sets the <see cref="Rigidbody.velocity"/> to the product of <see cref="currentDirection"/>
    /// and <see cref="moveSpeed"/>, preserving the Y axis for gravity.
    /// When there is no input, gradually reduces the velocity to 0.
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
    /// Handles player rotation.
    /// When there is input, sets the <see cref="Rigidbody.rotation"/> to smoothly rotate towards
    /// the direction of the input using <see cref="Quaternion.Slerp"/>.
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