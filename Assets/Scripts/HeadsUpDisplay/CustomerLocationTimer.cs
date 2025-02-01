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
            return;
        }
    }

    private void Update()
    {
        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            timerText.text = Mathf.Ceil(countdownTime).ToString(); // Display as whole number
        }
        else
        {
            timerText.text = "0"; // Ensure it doesn't go negative
            //Destroy(gameObject); // Optionally destroy the script/game object when time runs out
        }
    }
}