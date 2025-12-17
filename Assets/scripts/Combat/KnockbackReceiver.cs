using UnityEngine;
using System.Collections;

public class KnockbackReceiver : MonoBehaviour
{
    public float knockbackDuration = 0.12f;
    public float hitStopTime = 0.15f; // TEMP: big so you can see it

    private bool isKnockedBack = false;
    public bool IsKnockedBack => isKnockedBack;

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (!gameObject.activeInHierarchy) return;

        StopAllCoroutines();
        StartCoroutine(KnockbackRoutine(direction.normalized, force));
    }

    IEnumerator KnockbackRoutine(Vector2 direction, float force)
    {
        isKnockedBack = true;

        Debug.Log($"{name} HIT STOP");

        // Realtime pause (works even if timescale changes)
        if (hitStopTime > 0f)
            yield return new WaitForSecondsRealtime(hitStopTime);

        float timer = 0f;
        while (timer < knockbackDuration)
        {
            transform.position += (Vector3)(direction * force * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;
    }
}
