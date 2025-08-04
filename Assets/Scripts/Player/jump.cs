using UnityEngine;

public class jump : MonoBehaviour
{
    [Header("Jump Control")]
    public static float jumpingPower = 20f;
    public static int maxJumps = 1; // Maximum number of jumps allowed
    private int jumpsRemaining;

    [Header("Components")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck"); 
        jumpsRemaining = maxJumps; // Initially grant jumps for the maximum number of jumps allowed

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded() || jumpsRemaining > 0))
        {
            anim.SetBool("isJump", true);
            if (!isGrounded()) // If we are not on the ground, it means a double jump is being made.
            {
                jumpsRemaining--;
            }
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }

    private bool isGrounded() //Ground contact control
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGrounded())
        {
            jumpsRemaining = maxJumps;
            anim.SetBool("isJump", false);
        }
    }
}
