using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))] // Ensures PlayerController is on the same object
public class AutoFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PathGenerator pathGenerator;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool autoFollowOnStart = true; // Set this to true in Inspector to move after restart
    [SerializeField] private Animator animator;

    // --- FIX: Reference to the script we need to disable ---
    private PlayerController playerController;

    private List<Vector3> pathPoints;
    private int currentTargetIndex = 0;
    private SpriteRenderer spriteRenderer;
    private bool isFollowing = false;

    private void Awake()
    {
        // Get references to components on this GameObject
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null) animator = GetComponent<Animator>();

        if (pathGenerator == null) Debug.LogError("AutoFollower: PathGenerator reference not set!");
        if (playerController == null) Debug.LogError("AutoFollower: PlayerController component not found!");
    }

    private void Start()
    {
        if (autoFollowOnStart)
        {
            StartAutoFollow();
        }
    }

    private void Update()
    {
        // The Update loop only needs to run the FollowPath logic if it's active
        if (isFollowing)
        {
            FollowPath();
        }
    }

    private void FollowPath()
    {
        // Check if the path is finished or invalid
        if (pathPoints == null || currentTargetIndex >= pathPoints.Count)
        {
            Debug.Log("AutoFollower: Reached end of path.");
            StopAutoFollow(); // Cleanly stop and hand control back
            return;
        }
        
        Vector3 target = pathPoints[currentTargetIndex];
        Vector3 direction = (target - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target);
        
        if (distance > 0.01f) 
        {
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Flip sprite based on movement direction
            if (spriteRenderer != null)
            {
                if (direction.x > 0.05f)
                    spriteRenderer.flipX = false;
                else if (direction.x < -0.05f)
                    spriteRenderer.flipX = true;
            }

            // Update animator
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
            animator.SetBool("isMoving", true);
            animator.SetBool("isRunning", moveSpeed > 2f);
        }

        // Move to the next point in the path
        if (distance < 0.1f)
        {
            currentTargetIndex++;
        }
    }

    // Call this method to START the automated movement
    public void StartAutoFollow()
    {
        if (pathGenerator == null) return;

        Debug.Log("Starting Auto-Follow. Disabling PlayerController.");
        
        // --- FIX: Disable PlayerController ---
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Initialize path and state
        pathPoints = new List<Vector3>(pathGenerator.worldPathPoints);
        currentTargetIndex = 0; // Always start from the beginning of the path
        isFollowing = true;
    }

    // Call this method to STOP the automated movement
    public void StopAutoFollow()
    {
        Debug.Log("Stopping Auto-Follow. Re-enabling PlayerController.");

        isFollowing = false;

        // --- FIX: Tell the Animator to stop moving ---
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }

        // --- FIX: Re-enable PlayerController ---
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}