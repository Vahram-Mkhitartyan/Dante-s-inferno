using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Framing")]
    public Vector2 screenOffset = new Vector2(6f, 0.5f);

    [Header("Follow Feel")]
    [Tooltip("Higher = heavier camera")]
    public float followSmoothTime = 0.6f;

    [Tooltip("0–1. Lower = calmer vertical movement")]
    [Range(0f, 1f)]
    public float verticalFollowFactor = 0.4f;

    private Vector3 velocity;
    private Transform lastTarget;
    private Coroutine moveRoutine;

    void LateUpdate()
    {
        if (moveRoutine != null) return;
        if (!target) return;

        // Reset inertia if target changes (prevents oscillation buildup)
        if (target != lastTarget)
        {
            velocity = Vector3.zero;
            lastTarget = target;
        }

        // Desired framing position
        Vector3 desired = new Vector3(
            target.position.x + screenOffset.x,
            target.position.y + screenOffset.y,
            -10f
        );

        // Calm vertical response (prevents water jitter)
        desired.y = Mathf.Lerp(
            transform.position.y,
            desired.y,
            verticalFollowFactor
        );

        // Heavy cinematic smoothing
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            followSmoothTime
        );
    }

    public void MoveToTargetFraming(Vector3 targetPosition, float duration)
    {
        Vector3 desired = GetDesiredPosition(targetPosition);

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToRoutine(desired, duration));
    }

    Vector3 GetDesiredPosition(Vector3 targetPosition)
    {
        return new Vector3(
            targetPosition.x + screenOffset.x,
            targetPosition.y + screenOffset.y,
            -10f
        );
    }

    System.Collections.IEnumerator MoveToRoutine(Vector3 destination, float duration)
    {
        velocity = Vector3.zero;

        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, destination);
        float travelDuration = Mathf.Max(0.05f, duration);
        float speed = distance / travelDuration;
        float traveled = 0f;

        while (traveled < distance)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
            traveled += step;
            yield return null;
        }

        transform.position = destination;
        moveRoutine = null;
    }
}
