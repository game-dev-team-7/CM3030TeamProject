using UnityEngine;

public class GlobalSnowController : MonoBehaviour
{
    //Toggles the active state of the global snow effect.
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}