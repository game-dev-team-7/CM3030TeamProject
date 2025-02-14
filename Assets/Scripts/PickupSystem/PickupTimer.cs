using UnityEngine;

public class PickupTimer : MonoBehaviour
{
    private float timer;

    public void SetExpirationTime(float time)
    {
        timer = time;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    public bool IsExpired()
    {
        return timer <= 0;
    }
}