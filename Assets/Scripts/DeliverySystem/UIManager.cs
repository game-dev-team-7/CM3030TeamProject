using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;

    private DeliveryNotificationSounds deliveryNotificationSounds;



    private void Start()
    {
        InitializeUIReferences();
        deliveryNotificationSounds = FindObjectOfType<DeliveryNotificationSounds>();
    }

    private void InitializeUIReferences()
    {
        timerText = GameObject.FindGameObjectWithTag("CustomerTimer")?.GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.FindGameObjectWithTag("Score")?.GetComponent<TextMeshProUGUI>();
        streakText = GameObject.FindGameObjectWithTag("Streak")?.GetComponent<TextMeshProUGUI>();
    }

    public void UpdateTimer(float seconds)
    {
        if (timerText != null)
        {
            if (seconds > 5)
                timerText.text = "Timer: " + Mathf.FloorToInt(seconds).ToString();
            else
                timerText.text = "Timer: " + seconds.ToString("F1");
        }
    }

    public void ResetTimer()
    {
        if (timerText != null) timerText.text = "Timer: -";
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

        if (deliveryNotificationSounds != null)
        {
            deliveryNotificationSounds.PlaySuccessSound();
        }
    }

    public void ShowFailureMessage()
    {
        if (streakText != null)
        {
            streakText.text = "Task Failed";
            Invoke(nameof(ResetStreakText), 1.5f);
        }

        if (deliveryNotificationSounds != null)
        {
            deliveryNotificationSounds.PlayFailureSound();
        }
    }

    private void ResetStreakText()
    {
        if (streakText != null) streakText.text = "";
    }
}