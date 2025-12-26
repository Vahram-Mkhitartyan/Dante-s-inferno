using UnityEngine;
using TMPro;
using System.Collections;

public class WhisperManager : MonoBehaviour
{
    public static WhisperManager Instance;

    [Header("UI")]
    public TextMeshProUGUI whisperText;
    public float fadeInTime = 1f;
    public float displayTime = 3f;
    public float fadeOutTime = 1f;

    Coroutine currentRoutine;

    void Awake()
    {
        Instance = this;
        whisperText.alpha = 0f;
    }

    public void ShowWhisper(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(WhisperRoutine(message));
    }

    IEnumerator WhisperRoutine(string message)
    {
        whisperText.text = message;

        // Fade in
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            whisperText.alpha = t / fadeInTime;
            yield return null;
        }

        whisperText.alpha = 1f;
        yield return new WaitForSeconds(displayTime);

        // Fade out
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            whisperText.alpha = 1f - (t / fadeOutTime);
            yield return null;
        }

        whisperText.alpha = 0f;
    }
}
