using UnityEngine;

public class PlayerCombatInput : MonoBehaviour
{
    private AttackExecutor executor;
    private PlayerController player;

    void Awake()
    {
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
                executor.Execute(AttackType.TopDownSwing);
                break;

            case 'B':
                executor.Execute(AttackType.UpwardLauncher);
                break;

            case 'C':
                executor.Execute(AttackType.ForwardPierce);
                break;
        }
    }

}
