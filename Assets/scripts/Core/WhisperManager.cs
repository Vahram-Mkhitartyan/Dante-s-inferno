using UnityEngine;
using TMPro;
using System.Collections;

public class WhisperManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public float fadeTime = 0.5f;
    public float displayTime = 3f;

    Coroutine currentRoutine;

    public bool IsWhispering => currentRoutine != null;

    public void Whisper(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowText(message));
    }

    public bool TryWhisper(string message)
    {
        if (IsWhispering)
            return false;

        Whisper(message);
        return true;
    }

    IEnumerator ShowText(string message)
    {
        subtitleText.text = message;
        subtitleText.alpha = 0f;

        // Fade in
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            subtitleText.alpha = t / fadeTime;
            yield return null;
        }

        subtitleText.alpha = 1f;
        yield return new WaitForSeconds(displayTime);

        // Fade out
        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            subtitleText.alpha = 1f - (t / fadeTime);
            yield return null;
        }

        subtitleText.alpha = 0f;
        currentRoutine = null;
    }
}
