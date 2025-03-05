using UnityEngine;

public class PlayerSnowController : MonoBehaviour
{
    //Toggles the active state of the snow effect around the player.
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
