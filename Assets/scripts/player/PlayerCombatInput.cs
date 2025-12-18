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
        Debug.Log("PlayerCombatInput AWAKE on ");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            HandleInput('A');

        if (Input.GetKeyDown(KeyCode.K))
            HandleInput('B');

        if (Input.GetKeyDown(KeyCode.L) && player.IsGrounded)
            HandleInput('C');
    }


    void HandleInput(char input)
    {
        combo.Register(input);

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
