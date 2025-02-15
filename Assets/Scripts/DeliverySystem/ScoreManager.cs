using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score;
    private int streak;

    public int CurrentScore => score;
    public int CurrentStreak => streak;

    public void IncrementScore()
    {
        streak++;
        score += streak;
    }

    public void ResetStreak()
    {
        streak = 0;
    }
}