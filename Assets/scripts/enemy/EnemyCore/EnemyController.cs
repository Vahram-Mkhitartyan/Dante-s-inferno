using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform Target { get; private set; }

    private EnemyState state;
    private IEnemyMovement movement;
    private EnemyPerception perception;

    void Awake()
    {
        state = GetComponent<EnemyState>();
        movement = GetComponent<IEnemyMovement>();
        perception = GetComponent<EnemyPerception>();

        Target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!state || !state.IsHostile || Target == null)
            return;

        if (perception != null && !perception.CanSeePlayer())
            return;

        movement?.TickMovement(Target);
    }
}
