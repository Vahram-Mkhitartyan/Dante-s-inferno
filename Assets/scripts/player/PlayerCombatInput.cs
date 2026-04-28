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
            HandleInput('A');

        if (Input.GetKeyDown(KeyCode.K))
            HandleInput('B');

        if (Input.GetKeyDown(KeyCode.L) && player != null && player.IsGrounded)
            HandleInput('C');
    }


    void HandleInput(char input)
    {
        if (executor == null)
            return;

        switch (input)
        {
            case 'A':
                executor.Execute(AttackType.Sword);
                break;

            case 'B':
                executor.Execute(AttackType.KickLauncher);
                break;

            case 'C':
                executor.Execute(AttackType.Spin360);
                break;
        }
    }

}
