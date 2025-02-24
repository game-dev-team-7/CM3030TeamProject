using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;
    
    private void Start()
    {
        InitializeUIReferences();
    }

    private void InitializeUIReferences()
    {
        timerText = GameObject.FindGameObjectWithTag("CustomerTimer")?.GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.FindGameObjectWithTag("Score")?.GetComponent<TextMeshProUGUI>();
        streakText = GameObject.FindGameObjectWithTag("Streak")?.GetComponent<TextMeshProUGUI>();
    }

    public void UpdateTimer(int seconds)
    {
        if (timerText != null) timerText.text = seconds.ToString();
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    public void ShowSuccessMessage(int streak)
    {
        if (streakText != null)
        {
            streakText.text = "+" + streak;
            Invoke(nameof(ResetStreakText), 1.5f);
        }
    }

    public void ShowFailureMessage()
    {
        if (streakText != null)
        {
            streakText.text = "Task Failed";
            Invoke(nameof(ResetStreakText), 1.5f);
        }
    }

    private void ResetStreakText()
    {
        if (streakText != null) streakText.text = "";
    }
}