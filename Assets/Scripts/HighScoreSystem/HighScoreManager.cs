using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
///     Manages the saving, loading, and display of high scores.
///     Persists high score data between game sessions using JSON file storage.
/// </summary>
public class HighScoreManager : MonoBehaviour
{
    /// <summary>
    ///     Reference to the UI text element that displays the high score.
    /// </summary>
    [SerializeField] private TextMeshProUGUI highScoreText;

    /// <summary>
    ///     File path where the high score data is stored.
    /// </summary>
    private string filePath;

    /// <summary>
    ///     Initializes the high score manager when the object awakens.
    ///     Sets up the file path and updates the UI with the current high score.
    /// </summary>
    private void Awake()
    {
        filePath = Application.persistentDataPath + "/highscore.json";
        UpdateHighScoreUI();
    }

    /// <summary>
    ///     Saves a new high score if it's greater than the current one.
    /// </summary>
    /// <param name="score">The new score to potentially save as high score</param>
    public void SaveHighScore(int score)
    {
        var data = new HighScoreData();

        // Load existing high score
        var currentHighScore = LoadHighScore();

        // Only save if the new score is higher
        if (score > currentHighScore)
        {
            data.highScore = score;
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);
            Debug.Log("New High Score Saved: " + score);

            // Update the UI to reflect the new high score
            UpdateHighScoreUI();
        }
    }

    /// <summary>
    ///     Loads the current high score from persistent storage.
    /// </summary>
    /// <returns>The current high score, or 0 if no high score exists</returns>
    public int LoadHighScore()
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var data = JsonUtility.FromJson<HighScoreData>(json);
            return data.highScore;
        }

        return 0; // Default score if no file exists
    }

    /// <summary>
    ///     Updates the high score display in the UI.
    /// </summary>
    private void UpdateHighScoreUI()
    {
        if (highScoreText != null) highScoreText.text = $"{LoadHighScore()}";
    }
}