using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float distance = 5.0f;  // Distance behind the player
    public float height = 2.0f;  // Height above the player
    public float followSpeed = 5.0f;  // Speed at which the camera follows

    private Vector3 offset;  // Offset between camera and player

    void Start()
    {
        // Calculate the initial offset
        offset = new Vector3(0, height, -distance);
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate the target position
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Make the camera look at the player
        transform.LookAt(player.position + Vector3.up * height / 2.0f);
    }
}