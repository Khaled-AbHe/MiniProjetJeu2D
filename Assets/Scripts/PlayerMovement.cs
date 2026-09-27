using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 12f;

    [HideInInspector] public bool slowPlayer = false;

    // Set true by PlayerMagicAbilities while a Siphon Dash is in progress.
    // While true, normal horizontal movement is suspended so the dash has
    // full control of the Rigidbody's velocity.
    [HideInInspector] public bool isDashing = false;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;          // Empty child object at the player's feet
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    [HideInInspector] public bool isGrounded;
    private bool isFacingRight = true;

    public bool IsFacingRight => isFacingRight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Legacy input: "Horizontal" maps to A/D and Left/Right arrows by default
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump: "Jump" maps to Space by default
        if (Input.GetButtonDown("Jump") && isGrounded && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsMoving", Mathf.Abs(horizontalInput) > 0);

        if (!isDashing) HandleFlip();
    }

    private void FixedUpdate()
    {
        // While a Siphon Dash is active, PlayerMagicAbilities drives velocity directly.
        if (isDashing) return;

        // Move horizontally while preserving vertical velocity (gravity/jumping)
        float currSpeed = slowPlayer ? moveSpeed / 2 : moveSpeed;
        rb.linearVelocity = new Vector2(horizontalInput * currSpeed, rb.linearVelocity.y);
    }

    private void HandleFlip()
    {
        if (horizontalInput > 0f && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0f && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Facing2D.Flip(transform);
    }

    // Shows the ground check circle in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
