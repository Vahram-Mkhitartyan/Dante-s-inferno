using UnityEngine;

public class GigantesMovement : MonoBehaviour, IEnemyMovement
{
    public float moveSpeed = 1.1f;
    public float stopDistance = 1.6f;

    private GigantesAttack attack;

    void Awake()
    {
        attack = GetComponent<GigantesAttack>();
    }

    public void TickMovement(Transform target)
    {
        if (target == null) return;

        // If committed to an attack, DO NOTHING
        if (attack != null && attack.IsCommitted)
            return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= stopDistance)
            return;

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }
}
