using UnityEngine;

public class LightToggle : MonoBehaviour
{
    public GameObject spotLightObject;
    private Light spotLight;

    void Start()
    {
        if (spotLightObject != null)
        {
            spotLight = spotLightObject.GetComponent<Light>(); // Get the Light component
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && spotLight != null)
        {
            spotLight.enabled = !spotLight.enabled; // Toggle the light on/off
        }
    }
}
