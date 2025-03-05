using UnityEngine;

public class CameraControllerV2 : MonoBehaviour
{
    [Header("Target to follow")] public Transform target;

    //Assigning camera variables that can be modified in the inspector.
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
        //Camera initial setup
        currentHorizontal = camHorizontalAngle;
        currentVertical = camVerticalAngle;
        offset = new Vector3(0, camHeight, -camDistance);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1)) //When Right mouse button is held...
        {
            //Gets mouse coordinates inputs
            currentHorizontal += Input.GetAxis("Mouse X") * rotationSpeed;
            currentVertical -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentVertical = Mathf.Clamp(currentVertical, -5f, 70f); //This clamps vertical rotation(camera does not go under the map)
        }
    }

    private void LateUpdate()//Gets called after Update
    {
        if (target == null) return;

        //Smooth follow using Lerp
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 5f);

        //This applies rotations to the camera
        var rotation = Quaternion.Euler(currentVertical, currentHorizontal, 0);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target.position + Vector3.up * (camHeight * 0.5f));
    }
}