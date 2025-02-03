using UnityEngine;
using TMPro;

namespace DeliverySystem
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public TMP_Text scoreText;
        public TMP_Text streakText;
        public TMP_Text timerText;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public void UpdateUI()
        {
            scoreText.text = $"Score: {DeliveryManager.Instance.score}";
            streakText.text = $"Streak: x{DeliveryManager.Instance.streak}";
        }

        private void Update()
        {
            if (DeliveryManager.Instance.currentTask != null)
            {
                var timeRemaining = DeliveryManager.Instance.currentTask.expirationTime - Time.time;
                timerText.text = $"Time Remaining: {Mathf.Max(0, timeRemaining):F1}s";
            }
        }
    }
}