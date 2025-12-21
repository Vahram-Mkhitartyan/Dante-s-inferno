using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public CharacterAnimator animator;

    [SerializeField] private Transform characterGFX;
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public float gravity = 35f;
    public LayerMask groundLayer;

    public int FacingDirection { get; private set; } = -1;
    public Vector2 FacingDirectionVector => new Vector2(FacingDirection, 0f);

    public bool IsGrounded { get; private set; }
    public bool IsDefending { get; private set; }

    private Rigidbody2D rb;
    private Collider2D col;
    private KnockbackReceiver knockback;
    private Transform gfx;

    private bool isLocked;
    private PlayerAnimations currentAnim; // ✅ SINGLE source of truth

    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        knockback = GetComponent<KnockbackReceiver>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        animator = GetComponentInChildren<CharacterAnimator>();
        gfx = animator.transform;
    }

    void Update()
    {
        // 🔒 HARD LOCK (attacks / hurt / death)
        if (isLocked)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");

        // ---------- FACING ----------
        if (x != 0)
        {
            FacingDirection = (int)Mathf.Sign(x);

            Vector3 scale = gfx.localScale;
            scale.x = Mathf.Abs(scale.x) * FacingDirection;
            gfx.localScale = scale;
        }

        // ---------- INPUT ----------
        IsDefending = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.W) && IsGrounded && !IsDefending)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            IsGrounded = false;
        }

        // ---------- MOVEMENT ----------
        if (IsDefending)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // ---------- ANIMATION PRIORITY ----------
        if (IsDefending)
        {
            PlayAnim(PlayerAnimations.Special); // Defence
            return;
        }

        if (!IsGrounded)
        {
            PlayAnim(PlayerAnimations.FullJump);
        }
        else if (Mathf.Abs(x) > 0.01f)
        {
            PlayAnim(PlayerAnimations.Walk);
        }
        else
        {
            PlayAnim(PlayerAnimations.Idle);
        }
    }

    void PlayAnim(PlayerAnimations anim)
    {
        if (currentAnim == anim) return;
        currentAnim = anim;

        animator.ChangeAnimation(anim.ToString());
    }


    void FixedUpdate()
    {
        if (knockback && knockback.IsKnockedBack)
            return;

        // Gravity
        rb.linearVelocity += Vector2.down * gravity * Time.fixedDeltaTime;

        // Ground check
        IsGrounded = Physics2D.BoxCast(
            col.bounds.center,
            col.bounds.size,
            0f,
            Vector2.down,
            0.05f,
            groundLayer
        );
    }
}
