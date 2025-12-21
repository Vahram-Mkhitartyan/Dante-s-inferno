using UnityEngine;
using Spine;
using Spine.Unity;

public class PlayerSpineAnimationController : MonoBehaviour
{
    private SkeletonAnimation skeleton;
    private Spine.AnimationState animState;

    private bool isLocked;
    private bool isDead;

    void Awake()
    {
        skeleton = GetComponent<SkeletonAnimation>();
        animState = skeleton.AnimationState;
    }

    // ===== Movement =====
    public void SetIdle()
    {
        if (isDead || isLocked) return;
        animState.SetAnimation(0, "Idle", true);
    }

    public void SetMove(float x)
    {
        if (isDead || isLocked) return;

        if (Mathf.Abs(x) > 0.01f)
            animState.SetAnimation(0, "Walk", true);
        else
            SetIdle();
    }

    // ===== Actions =====
    public void RequestAttack(string animName, float duration)
    {
        if (isDead || isLocked) return;

        isLocked = true;
        animState.SetAnimation(0, animName, false);
        Invoke(nameof(Unlock), duration);
    }

    public void RequestDeath()
    {
        if (isDead) return;

        isDead = true;
        isLocked = true;

        CancelInvoke();
        animState.ClearTracks();
        animState.SetAnimation(0, "Death", false);
    }


    public void RequestHurt(float duration)
    {
        if (isDead) return;

        CancelInvoke(nameof(Unlock));
        isLocked = true;

        animState.SetAnimation(0, "Hurt", false);
        Invoke(nameof(Unlock), duration);
        Debug.Log("HURT CALLED. isDead = " + isDead);
    }


    public bool IsLocked() => isLocked;
    public bool IsDead() => isDead;
    

    private void Unlock()
    {
        if (isDead) return;
        isLocked = false;
    }
}
