using UnityEngine;

/// <summary>
///     Applies continuous rotation to a GameObject around a specified axis.
///     Typically used for buff pickup objects to make them visually appealing.
/// </summary>
public class BuffBodyAnimation : MonoBehaviour
{
    /// <summary>
    ///     The axis around which the object rotates.
    ///     Default is Vector3.up (y-axis).
    /// </summary>
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    /// <summary>
    ///     Speed of rotation in degrees per second.
    /// </summary>
    [SerializeField] private float rotationSpeed = 50f;

    /// <summary>
    ///     Called once per frame. Applies the rotation based on delta time.
    /// </summary>
    private void Update()
    {
        // Apply rotation every frame
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}