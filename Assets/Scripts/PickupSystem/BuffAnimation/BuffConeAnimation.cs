using UnityEngine;

/// <summary>
///     Animates a buff cone object with vertical movement and pulsing emission.
///     Creates a floating effect with synchronized material emission changes.
/// </summary>
public class BuffConeAnimation : MonoBehaviour
{
    /// <summary>
    ///     Property name for accessing the emission color in the shader.
    /// </summary>
    private static readonly string emissionProperty = "_EmissionColor";

    /// <summary>
    ///     Height of the vertical movement in units.
    /// </summary>
    [SerializeField] private float amplitude = 1f;

    /// <summary>
    ///     Speed of the vertical movement cycle.
    /// </summary>
    [SerializeField] private float speed = 2f;

    /// <summary>
    ///     Minimum emission intensity for the pulsing effect.
    /// </summary>
    [SerializeField] private float minEmission;

    /// <summary>
    ///     Maximum emission intensity for the pulsing effect.
    /// </summary>
    [SerializeField] private float maxEmission = 3f;

    /// <summary>
    ///     Reference to the object's material for emission control.
    /// </summary>
    private Material objectMaterial;

    /// <summary>
    ///     The initial position of the object when the script starts.
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    ///     Called when the script instance is being loaded.
    ///     Initializes starting position and material references.
    /// </summary>
    private void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;

        // Get the object's material
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            objectMaterial = renderer.material;

            // Ensure emission is enabled on the material
            objectMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogWarning("No Renderer component found on " + gameObject.name + ". Emission effects will not work.");
        }
    }

    /// <summary>
    ///     Called once per frame. Updates the object's position and emission intensity.
    /// </summary>
    private void Update()
    {
        // Calculate new Y position based on sine wave
        var newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Dynamically calculate min and max Y based on amplitude
        var minY = startPosition.y - amplitude;
        var maxY = startPosition.y + amplitude;

        // Normalize Y position to a 0-1 range for emission control
        var normalizedY = Mathf.InverseLerp(minY, maxY, newY);

        // Calculate new emission intensity based on the normalized position
        var emissionIntensity = Mathf.Lerp(minEmission, maxEmission, normalizedY);

        // Apply the emission intensity to the material
        if (objectMaterial != null)
        {
            var baseEmissionColor = Color.white; // Base emission color
            objectMaterial.SetColor(emissionProperty, baseEmissionColor * emissionIntensity);
        }
    }

    /// <summary>
    ///     Called when the script is being destroyed.
    ///     Ensures proper cleanup of material resources.
    /// </summary>
    private void OnDestroy()
    {
        // If we created a material instance, we should clean it up
        if (objectMaterial != null && !Application.isPlaying)
            // In edit mode, destroy the material instance to prevent leaks
            DestroyImmediate(objectMaterial);
    }
}