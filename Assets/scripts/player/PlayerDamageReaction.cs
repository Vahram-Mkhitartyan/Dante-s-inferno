using UnityEngine;

public class PlayerDamageReaction : MonoBehaviour
{
    private Health health;
    private PlayerSpineAnimationController anim;
    private PlayerController controller;

    [SerializeField] private float hurtLockTime = 0.4f;

    void Awake()
    {
        health = GetComponent<Health>();
        anim = GetComponentInChildren<PlayerSpineAnimationController>();
        controller = GetComponent<PlayerController>();

        health.OnDamaged += OnHurt;
        health.OnDeath += OnDeath;
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

    void Unlock()
    {
        if (health.IsDead) return;
        controller.SetLocked(false);
    }
}
