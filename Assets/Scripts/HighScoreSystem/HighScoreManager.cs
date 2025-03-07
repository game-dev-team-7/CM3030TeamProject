using System.IO;
using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    private string filePath;
    [SerializeField] private TextMeshProUGUI highScoreText;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/highscore.json";
        UpdateHighScoreUI();
    }

    public void SaveHighScore(int score)
    {
        HighScoreData data = new HighScoreData();

        // Load existing high score
        int currentHighScore = LoadHighScore();

        // Only save if the new score is higher
        if (score > currentHighScore)
        {
            data.highScore = score;
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);
            Debug.Log(" Current High Score: " + score);
        }
    }
    public int LoadHighScore()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
            return data.highScore;
        }
        
        return 0; // Default score if no file exists
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"{LoadHighScore()}";
        }
    }
}