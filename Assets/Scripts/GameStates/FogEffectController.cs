using UnityEngine;

public class FogEffectController : MonoBehaviour
{
    // Toggles the active state of the fog effect.
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}