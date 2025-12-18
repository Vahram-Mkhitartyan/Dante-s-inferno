using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public float gravity = 35f;
    public float skinWidth = 0.02f;
    public LayerMask groundLayer;

    public int FacingDirection { get; private set; } = 1;
    public bool IsGrounded => isGrounded;

    private Vector2 velocity;
    private bool isGrounded;

    private SpriteRenderer sprite;
    private Collider2D col;
    private KnockbackReceiver knockback;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        knockback = GetComponent<KnockbackReceiver>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        velocity.x = x * moveSpeed;

        if (x != 0)
        {
            FacingDirection = (int)Mathf.Sign(x);
            sprite.flipX = FacingDirection == -1;
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
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
        transform.position += Vector3.right * velocity.x * dt;

        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,
            col.bounds.size,
            0f,
            Vector2.down,
            skinWidth,
            groundLayer
        );

        if (hit && velocity.y <= 0)
        {
            isGrounded = true;
            velocity.y = 0;

            transform.position = new Vector3(
                transform.position.x,
                hit.point.y + col.bounds.extents.y,
                transform.position.z
            );
        }
        else
        {
            isGrounded = false;
            velocity.y -= gravity * dt;
            transform.position += Vector3.up * velocity.y * dt;
        }
    }
}
