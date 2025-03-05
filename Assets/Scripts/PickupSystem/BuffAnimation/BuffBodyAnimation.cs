using UnityEngine;

public class BuffBodyAnimation : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationAxis = Vector3.up; // Rotate around the Y-axis by default
    [SerializeField]
    private float rotationSpeed = 50f; // Speed of rotation (degrees per second)

    void Update()
    {
        // Apply rotation every frame
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}