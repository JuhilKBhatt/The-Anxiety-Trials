using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float acceleration = 10f;

    [Header("State Flags")]
    [SerializeField] private bool canMove = true;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private bool isRunning;
    private bool isHurt;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0f; // Ensure gravity is disabled for top-down movement
    }

    private void Update()
    {
        if (isDead) return;

        HandleInput();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isDead || !canMove) return;

        MovePlayer();
    }

    /// <summary>
    /// Reads player input and updates movement state.
    /// </summary>
    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;
        isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    /// <summary>
    /// Smoothly moves the player in 2D space (top-down).
    /// </summary>
    private void MovePlayer()
    {
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        Vector2 targetVelocity = moveInput * targetSpeed;

        // Smooth acceleration / deceleration
        currentVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = currentVelocity;

        // ✅ Flip sprite horizontally
        if (moveInput.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    /// <summary>
    /// Updates animation parameters.
    /// </summary>
    private void UpdateAnimator()
    {
        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
        animator.SetBool("isMoving", moveInput.magnitude > 0.1f);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isHurt", isHurt);
        animator.SetBool("isDead", isDead);
    }

    #region Public Methods

    public void TakeDamage()
    {
        if (isDead) return;
        isHurt = true;
        animator.SetTrigger("Hurt");
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");
        canMove = false;
    }

    #endregion
}