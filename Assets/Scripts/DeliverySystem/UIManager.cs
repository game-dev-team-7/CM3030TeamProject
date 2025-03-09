using UnityEngine;
using TMPro;

/// <summary>
/// Manages all UI elements related to the delivery gameplay system.
/// Handles updates for timer display, score, streak notifications,
/// and success/failure messages.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;

    private DeliveryNotificationSounds deliveryNotificationSounds;

    /// <summary>
    /// Initializes UI references and sound components.
    /// </summary>
    private void Start()
    {
        InitializeUIReferences();
        deliveryNotificationSounds = FindObjectOfType<DeliveryNotificationSounds>();
    }

    /// <summary>
    /// Finds and caches references to UI text components by tag.
    /// </summary>
    private void InitializeUIReferences()
    {
        timerText = GameObject.FindGameObjectWithTag("CustomerTimer")?.GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.FindGameObjectWithTag("Score")?.GetComponent<TextMeshProUGUI>();
        streakText = GameObject.FindGameObjectWithTag("Streak")?.GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Updates the delivery timer display.
    /// Uses different formatting for urgent (<=5 seconds) and normal times.
    /// </summary>
    /// <param name="seconds">Remaining time in seconds</param>
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

    /// <summary>
    /// Resets the timer display when no delivery is active.
    /// </summary>
    public void ResetTimer()
    {
        if (timerText != null) timerText.text = "Timer: -";
    }

    /// <summary>
    /// Updates the score display with the current score.
    /// </summary>
    /// <param name="score">The current total score</param>
    public void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    /// <summary>
    /// Displays a success message with the current streak value
    /// and plays the corresponding sound effect.
    /// </summary>
    /// <param name="streak">The current streak count</param>
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

    /// <summary>
    /// Displays a failure message when a delivery times out
    /// and plays the corresponding sound effect.
    /// </summary>
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

    /// <summary>
    /// Clears the streak text after displaying a notification.
    /// </summary>
    private void ResetStreakText()
    {
        if (streakText != null) streakText.text = "";
    }
}