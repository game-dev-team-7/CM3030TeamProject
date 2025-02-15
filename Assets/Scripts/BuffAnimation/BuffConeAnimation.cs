using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffConeAnimation : MonoBehaviour
{
    [SerializeField]
    private float amplitude = 1f; // Height of the movement
    [SerializeField]
    private float speed = 2f;     // Speed of the movement

    [SerializeField]
    private float minEmission = 0f;  // Minimum emission intensity
    [SerializeField]
    private float maxEmission = 3f;  // Maximum emission intensity

    private Vector3 startPosition;
    private Material objectMaterial;
    private static readonly string emissionProperty = "_EmissionColor";

    void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;

        // Get the object's material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            objectMaterial = renderer.material;
            objectMaterial.EnableKeyword("_EMISSION"); // Ensure emission is enabled
        }
    }

    void Update()
    {
        // Calculate new Y position based on sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Dynamically calculate min and max Y based on amplitude
        float minY = startPosition.y - amplitude;
        float maxY = startPosition.y + amplitude;

        // Normalize Y position to a 0-1 range
        float normalizedY = Mathf.InverseLerp(minY, maxY, newY);

        // Calculate new emission intensity
        float emissionIntensity = Mathf.Lerp(minEmission, maxEmission, normalizedY);

        // Apply the emission intensity to the material
        if (objectMaterial != null)
        {
            Color baseEmissionColor = Color.white; // You can change this color
            objectMaterial.SetColor(emissionProperty, baseEmissionColor * emissionIntensity);
        }
    }
}