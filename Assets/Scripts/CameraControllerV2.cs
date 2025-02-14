using UnityEngine;

public class CameraControllerV2 : MonoBehaviour
{
    [Header("Target to follow")] public Transform target;


    [Header("Camera settings")] public float rotationSpeed = 3f;
    public float camDistance = 60f;
    public float camHeight = 20f;
    public float camHorizontalAngle = 0f;
    public float camVerticalAngle = 0f;

    private float currentHorizontal;
    private float currentVertical;
    private Vector3 offset;
    private Vector3 velocity;

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
            currentHorizontal += Input.GetAxis("Mouse X") * rotationSpeed;
            currentVertical -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentVertical = Mathf.Clamp(currentVertical, -80f, 80f); // Clamping vertical rotation
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow using Lerp instead of SmoothDamp
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 5f);

        // Apply rotations
        var rotation = Quaternion.Euler(currentVertical, currentHorizontal, 0);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target.position + Vector3.up * (camHeight * 0.5f));
    }
}