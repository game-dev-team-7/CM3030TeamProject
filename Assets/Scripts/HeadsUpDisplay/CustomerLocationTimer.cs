using UnityEngine;
using TMPro;

public class CustomerLocationTimer : MonoBehaviour
{
    [SerializeField] private float countdownTime = 30f; // Timer duration
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the TextMeshPro UI element

    private void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("TimerText (TextMeshProUGUI) is not assigned to CustomerLocationTimer!");
            enabled = false; // Disable the script to prevent unnecessary Update() calls
        }
    }

    private void Update()
    {
        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            timerText.text = Mathf.Ceil(Mathf.Max(0, countdownTime)).ToString(); // Prevent negative values
        }
    }
}