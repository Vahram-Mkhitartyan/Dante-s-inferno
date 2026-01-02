using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform Target { get; private set; }

    private EnemyState state;
    private IEnemyMovement movement;
    private IEnemyAttack attack;

    void Awake()
    {
        state = GetComponent<EnemyState>();
        movement = GetComponent<IEnemyMovement>();
        attack = GetComponent<IEnemyAttack>();

        Target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!state || !state.IsHostile || Target == null)
            return;

        movement?.TickMovement(Target);
        attack?.TryAttack(Target);
    }
}
