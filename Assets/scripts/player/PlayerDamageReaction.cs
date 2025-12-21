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

        health.OnDamaged += OnHurt;
        health.OnDeath += OnDeath;
        health.OnBlocked += OnBlock;
    }

    void OnHurt()
    {
        if (health.IsDead) return;

        anim.RequestHurt(hurtLockTime);
        controller.SetLocked(true);

        Invoke(nameof(Unlock), hurtLockTime);
    }

    void OnDeath()
    {
        anim.RequestDeath();
        controller.SetLocked(true);

        // hard stop
        CancelInvoke();
        health.OnDamaged -= OnHurt;
        health.OnDeath -= OnDeath;
    }
    void OnBlock()
    {
        controller.SetLocked(true);

        anim.RequestBlock(blockLockTime);

        Invoke(nameof(Unlock), blockLockTime);
    }

    void Unlock()
    {
        if (health.IsDead) return;
        controller.SetLocked(false);
    }
}
