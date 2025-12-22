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
    private PlayerSpineAnimationController spineAnim;

    private bool isLocked;
    private PlayerInputReader inputReader;
    private PlayerAnimationDriver animationDriver;

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
        spineAnim = GetComponentInChildren<PlayerSpineAnimationController>();

        inputReader = new PlayerInputReader();
        animationDriver = new PlayerAnimationDriver(animator);
    }

    void Update()
    {
        // 🔒 HARD LOCK (attacks / hurt / death)
        if (isLocked || (spineAnim && spineAnim.IsLocked()))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        PlayerInputState input = inputReader.Read();
        float x = input.MoveX;

        // ---------- FACING ----------
        if (x != 0)
        {
            FacingDirection = (int)Mathf.Sign(x);

            Vector3 scale = gfx.localScale;
            scale.x = Mathf.Abs(scale.x) * FacingDirection;
            gfx.localScale = scale;
        }

        // ---------- INPUT ----------
        IsDefending = input.DefendHeld;

        if (input.JumpPressed && IsGrounded && !IsDefending)
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
        animationDriver.UpdateAnimation(IsDefending, IsGrounded, x);
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
