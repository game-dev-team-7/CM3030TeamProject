using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeatherTintController : MonoBehaviour
{
    public Image tintImage; // Assign in Inspector
    public float transitionDuration = 1f; // Duration for tint transitions
    public AnimationCurve transitionCurve = AnimationCurve.Linear(0, 0, 1, 1); // Default linear

    private Coroutine currentTransition;

    void Start()
    {
        if (tintImage == null)
        {
            tintImage = GetComponent<Image>();
        }
    }

    /// <summary>
    /// Changes the screen tint to the specified color smoothly over transitionDuration using transitionCurve.
    /// </summary>
    /// <param name="newColor">The target color for the tint.</param>
    public void ChangeTint(Color newColor)
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }
        currentTransition = StartCoroutine(TransitionTint(newColor));
    }

    private IEnumerator TransitionTint(Color targetColor)
    {
        Color initialColor = tintImage.color;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            float curveValue = transitionCurve.Evaluate(t);
            tintImage.color = Color.Lerp(initialColor, targetColor, curveValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tintImage.color = targetColor;
    }
}