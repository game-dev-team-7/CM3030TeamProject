using UnityEngine;

public class DeliveryTimer
{
    private float timer;

    public float RemainingTime => timer;

    public void StartTimer(float duration)
    {
        timer = duration;
    }

    public void UpdateTimer()
    {
        if (timer > 0) timer -= Time.deltaTime;
    }

    public bool IsTimeExpired()
    {
        return timer <= 0;
    }
}