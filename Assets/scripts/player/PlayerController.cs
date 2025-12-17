using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;

    [Header("Grounding")]
    public float skinWidth = 0.02f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float gravity = 35f;
    public LayerMask groundLayer;

    private KnockbackReceiver knockback;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    public int FacingDirection { get; private set; } = 1;

    private Vector2 velocity;
    private bool isGrounded;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<KnockbackReceiver>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        float x = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x = 1f;

        if (x != 0)
        {
            FacingDirection = (int)Mathf.Sign(x);
            spriteRenderer.flipX = FacingDirection == -1;
        }

        velocity.x = x * moveSpeed;

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
        }
    }

    void LateUpdate()
    {
        if (knockback && knockback.IsKnockedBack)
            return;

        float dt = Time.deltaTime;

        // ----- HORIZONTAL MOVE -----
        transform.position += Vector3.right * velocity.x * dt;

        // ----- GROUND CHECK -----
        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,
            col.bounds.size,
            0f,
            Vector2.down,
            skinWidth,
            groundLayer
        );

        if (hit.collider != null && velocity.y <= 0f)
        {
            // Grounded: hard lock
            isGrounded = true;
            velocity.y = 0f;

            float groundY = hit.point.y + col.bounds.extents.y;
            transform.position = new Vector3(
                transform.position.x,
                groundY,
                transform.position.z
            );
        }
        else
        {
            // Airborne: gravity applies
            isGrounded = false;
            velocity.y -= gravity * dt;
            transform.position += Vector3.up * velocity.y * dt;
        }
    }
}
