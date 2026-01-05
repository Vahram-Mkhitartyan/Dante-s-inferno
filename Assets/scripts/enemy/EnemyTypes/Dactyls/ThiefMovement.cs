using UnityEngine;

public class ThiefMovement : MonoBehaviour, IEnemyMovement
{
    public float moveSpeed = 0.8f;
    public float fleeSpeedMultiplier = 1.8f;
    public float stopDistance = 1.0f;

    private float fleeUntilTime = -1f;

    public void TriggerFlee(float duration)
    {
        fleeUntilTime = Time.time + duration;
        Debug.Log("[ThiefMovement] 🏃 Flee triggered");
    }

    public void TickMovement(Transform target)
    {
        if (target == null) return;

        // FLEE
        if (Time.time < fleeUntilTime)
        {
            Vector2 away = ((Vector2)transform.position - (Vector2)target.position).normalized;
            transform.position += (Vector3)(away * moveSpeed * fleeSpeedMultiplier * Time.deltaTime);
            return;
        }

        // NORMAL MOVE
        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= stopDistance) return;

        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }
}
