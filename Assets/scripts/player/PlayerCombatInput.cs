using UnityEngine;

public class PlayerCombatInput : MonoBehaviour
{
    private ComboQueue combo;
    private AttackExecutor executor;
    private PlayerController player;

    void Awake()
    {
        combo = GetComponent<ComboQueue>();
        executor = GetComponent<AttackExecutor>();
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            combo.Register('A');

        if (Input.GetKeyDown(KeyCode.K))
            combo.Register('B');

        if (Input.GetKeyDown(KeyCode.L) && player.IsGrounded)
            combo.Register('C');

        ResolveCombos();
    }

    void ResolveCombos()
    {
        // C — Spin (ground-only)
        if (combo.Matches("C") && player.IsGrounded)
        {
            executor.Execute(AttackType.Spin360);
            combo.Clear();
            return;
        }

        // B — Kick
        if (combo.Matches("B"))
        {
            executor.Execute(AttackType.KickLauncher);
            combo.Clear();
            return;
        }

        // A — Sword
        if (combo.Matches("A"))
        {
            executor.Execute(AttackType.Sword);
            combo.Clear();
            return;
        }

        // Future combos go BELOW single attacks
        // Example:
        // if (combo.Matches("BA")) { ... }
    }
}
