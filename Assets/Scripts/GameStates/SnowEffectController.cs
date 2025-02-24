using UnityEngine;

public class SnowEffectController : MonoBehaviour
{
    // Toggles the active state of the snow effect.
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}