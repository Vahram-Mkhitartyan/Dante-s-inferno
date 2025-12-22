using UnityEngine;

public class PlayerAnimationDriver
{
    private readonly CharacterAnimator animator;
    private PlayerAnimations currentAnim;

    public PlayerAnimationDriver(CharacterAnimator animator)
    {
        this.animator = animator;
    }

    public void UpdateAnimation(bool isDefending, bool isGrounded, float moveX)
    {
        if (animator == null) return;

        PlayerAnimations nextAnim;
        if (isDefending)
        {
            nextAnim = PlayerAnimations.Special;
        }
        else if (!isGrounded)
        {
            nextAnim = PlayerAnimations.FullJump;
        }
        else if (Mathf.Abs(moveX) > 0.01f)
        {
            nextAnim = PlayerAnimations.Walk;
        }
        else
        {
            nextAnim = PlayerAnimations.Idle;
        }

        if (currentAnim == nextAnim) return;
        currentAnim = nextAnim;

        animator.ChangeAnimation(nextAnim.ToString());
    }
}
