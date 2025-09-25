using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Movement input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        // Example state conditions (replace with your own gameplay logic)
        bool isJumping = Input.GetKey(KeyCode.Space);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isHurt = false; // set from damage system
        bool isDead = false; // set from health system

        // Animator parameters
        anim.SetFloat("moveX", input.x);
        anim.SetFloat("moveY", input.y);
        anim.SetBool("isMoving", input.magnitude > 0.1f);
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isHurt", isHurt);
        anim.SetBool("isDead", isDead);
    }

    void FixedUpdate()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.MovePosition(rb.position + input * currentSpeed * Time.fixedDeltaTime);
    }
}