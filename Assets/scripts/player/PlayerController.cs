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
    public bool IsGrounded { get; private set; }
    private bool wasGrounded;
    private string currentAnim;
    private Transform gfx;  
    private Rigidbody2D rb;
    private Collider2D col;
    private KnockbackReceiver knockback;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        knockback = GetComponent<KnockbackReceiver>();

        rb.gravityScale = 0; // we control gravity manually
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        animator = GetComponentInChildren<CharacterAnimator>();
        gfx = animator.transform;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");


        if (x != 0)
        {
            FacingDirection = (int)Mathf.Sign(x);

            Vector3 scale = gfx.localScale;
            scale.x = Mathf.Abs(scale.x) * FacingDirection;
            gfx.localScale = scale;
        }

        if (Input.GetKeyDown(KeyCode.W) && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            IsGrounded = false;
        }

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // --- ANIMATION LOGIC (CORRECT) ---
        if (!IsGrounded)
        {
            PlayAnim("FullJump");
        }
        else if (Mathf.Abs(x) > 0.01f)
        {
            PlayAnim("Walk");
        }
        else
        {
            PlayAnim("Idle");
        }

    }

    void PlayAnim(string anim)
    {
        if (currentAnim == anim) return;
        currentAnim = anim;
        animator.ChangeAnimation(anim);
    }

    void FixedUpdate()
    {
        if (knockback && knockback.IsKnockedBack)
            return;

        // gravity
        rb.linearVelocity += Vector2.down * gravity * Time.fixedDeltaTime;

        // ground check
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
