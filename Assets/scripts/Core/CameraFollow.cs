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

    void LateUpdate()
    {
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
}
