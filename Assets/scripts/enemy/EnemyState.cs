using UnityEngine;

public class EnemyState : MonoBehaviour
{
    public bool IsHostile { get; private set; } = false;

    private EnemyMovement move;
    private EnemyAttack attack;

    void Awake()
    {
        move = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
    }

    public void SetHostile(bool hostile)
    {
        IsHostile = hostile;

        if (move) move.enabled = hostile;
        if (attack) attack.enabled = hostile;
    }
}
