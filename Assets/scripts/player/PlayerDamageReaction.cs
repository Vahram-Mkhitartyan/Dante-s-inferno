using UnityEngine;

public class PlayerDamageReaction : MonoBehaviour
{
    private Health health;


    [SerializeField] private float hurtLockTime = 0.4f;
    [SerializeField] private PlayerSpineAnimationController anim;
    [SerializeField] private PlayerController controller;
    [SerializeField] private float blockLockTime = 0.25f;

    void Awake()
    {
        health = GetComponent<Health>();
        anim = GetComponentInChildren<PlayerSpineAnimationController>();
        controller = GetComponent<PlayerController>();

        if (health == null || anim == null || controller == null)
            return;

        // Hook into health events for hurt, death, and block reactions.
        health.OnDamaged += OnHurt;
        health.OnDeath += OnDeath;
        health.OnBlocked += OnBlock;
    }

    void OnDestroy()
    {
        if (health == null) return;

        health.OnDamaged -= OnHurt;
        health.OnDeath -= OnDeath;
        health.OnBlocked -= OnBlock;
    }

    void OnHurt()
    {
        if (anim == null || controller == null) return;
        if (health.IsDead) return;

        // Lock player input briefly while playing the hurt animation.
        anim.RequestHurt(hurtLockTime);
        controller.SetLocked(true);

        Invoke(nameof(Unlock), hurtLockTime);
    }

    void OnDeath()
    {
        if (anim == null || controller == null) return;

        anim.RequestDeath();
        controller.SetLocked(true);

        // Hard stop any pending unlocks. Keep event handlers for future respawns.
        CancelInvoke();
    }
    void OnBlock()
    {
        if (anim == null || controller == null) return;

        controller.SetLocked(true);

        // Short lock/animation for a successful block.
        anim.RequestBlock(blockLockTime);

        Invoke(nameof(Unlock), blockLockTime);
    }

    void Unlock()
    {
        if (controller == null) return;
        if (health.IsDead) return;
        controller.SetLocked(false);
    }
}
