using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10.0f;   // Base forward movement speed
    public float turnSpeed = 100.0f; // Turning speed
    public float acceleration = 5.0f; // Acceleration rate
    public float deceleration = 5.0f; // Deceleration rate
    public float maxSpeed = 20.0f;    // Maximum speed
    public float minSpeed = 0.0f;     // Minimum speed (can be 0 or negative for reverse)

    private float currentSpeed = 0.0f; // Current forward speed

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Handle forward/backward movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }
        else
        {
            // Gradual slow down when no input
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        // Clamp the current speed between min and max speed
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

        // Handle turning (only while moving forward or backward)
        if (currentSpeed != 0)
        {
            float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
            transform.Rotate(0, turn, 0);
        }

        // Apply movement based on the current speed
        //transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        rb.velocity = transform.forward * currentSpeed;
    }
}
