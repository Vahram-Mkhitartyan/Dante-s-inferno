using UnityEngine;
using System.Collections;

public class PlotManager : MonoBehaviour
{
    [Header("References")]
    public WhisperManager whisper; // drag your whisper manager here

    [Header("Timing")]
    public float startDelay = 1.5f;

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        yield return new WaitForSeconds(startDelay);

        // Line 1
        whisper.Whisper("In twilight’s hush where neither sin nor flame,");
        yield return new WaitForSeconds(3.5f);

        // Line 2
        whisper.Whisper("The souls drift slow beside the silent stream,");
        yield return new WaitForSeconds(4f);

        // Line 3
        whisper.Whisper("Unjudged by fire, untouched by guilt or blame.");
        yield return new WaitForSeconds(3f);

        // Line 4
        whisper.Whisper("No cry is heard, no prayer escapes the dream,");
        yield return new WaitForSeconds(3f);

        whisper.Whisper("For hope was born too late, and faith too soon—");
        yield return new WaitForSeconds(3f);

        whisper.Whisper("They wait, unnamed, where shadows softly gleam.");
        yield return new WaitForSeconds(3f);



    }
}
