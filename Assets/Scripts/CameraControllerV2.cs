using UnityEngine;

public class CameraControllerV2 : MonoBehaviour
{
    [Header("Target to follow")] public Transform target;

    [Header("Camera settings")]
    public float rotationSpeed = 3f;
    public float camDistance = 60f;
    public float camHeight = 20f;
    public float camHorizontalAngle = 0f;
    public float camVerticalAngle = 0f;
    public float smoothTime = 0.1f; // Smoothness factor

    private float currentHorizontal;
    private float currentVertical;
    private float smoothHorizontal;
    private float smoothVertical;
    private float horizontalVelocity;
    private float verticalVelocity;

    private Vector3 offset;

    private void Start()
    {
        currentHorizontal = camHorizontalAngle;
        currentVertical = camVerticalAngle;
        offset = new Vector3(0, camHeight, -camDistance);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1)) // Right mouse button held
        {
             // Hide cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Handle camera rotation logic
            currentHorizontal += Input.GetAxis("Mouse X") * rotationSpeed;
            currentVertical -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentVertical = Mathf.Clamp(currentVertical, -5f, 70f);
        } else {
            // Show cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Smooth out the camera rotation using Mathf.SmoothDamp
        smoothHorizontal = Mathf.SmoothDamp(smoothHorizontal, currentHorizontal, ref horizontalVelocity, smoothTime);
        smoothVertical = Mathf.SmoothDamp(smoothVertical, currentVertical, ref verticalVelocity, smoothTime);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 5f);

        // Apply smooth rotation
        Quaternion rotation = Quaternion.Euler(smoothVertical, smoothHorizontal, 0);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target.position + Vector3.up * (camHeight * 0.5f));
    }
}
