using System.Collections.Generic;
using UnityEngine;

public class AutoFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PathGenerator pathGenerator;   // Reference to your PathGenerator
    [SerializeField] private float moveSpeed = 2f;          // Units per second
    [SerializeField] private bool autoFollow = false;       // Toggle ON/OFF

    [SerializeField] private Animator animator;


    private List<Vector3> pathPoints;
    private int currentTargetIndex = 0;

    private void Start()
    {
        if (pathGenerator != null)
        {
            pathPoints = new List<Vector3>(pathGenerator.worldPathPoints);
            Debug.Log($"AutoFollower: Loaded {pathPoints.Count} path points.");
        }
        else
        {
            Debug.LogError("AutoFollower: PathGenerator reference not set!");
        }
    }

    private void Update()
    {
        if (!autoFollow) return;

        // If pathPoints is null or empty, try to get them again
        if ((pathPoints == null || pathPoints.Count == 0) && pathGenerator != null)
        {
            pathPoints = new List<Vector3>(pathGenerator.worldPathPoints);
            if (pathPoints.Count > 0)
            {
                Debug.Log($"AutoFollower: Loaded {pathPoints.Count} path points.");
            }
            else
            {
                Debug.LogWarning("AutoFollower: Waiting for path points to be generated...");
                return;
            }
        }

        FollowPath();
    }

    private void FollowPath()
    {
        if (currentTargetIndex >= pathPoints.Count)
        {
            animator.SetBool("isMoving", false);
            autoFollow = false; // Stop at the end
            return;
        }

        Vector3 target = pathPoints[currentTargetIndex];
        Vector3 direction = (target - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target);

        // Move towards target
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Update animator
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
        animator.SetBool("isMoving", true);
        animator.SetBool("isRunning", moveSpeed > 2f); // Example: running if speed > 2

        // Snap to target if close enough
        if (distance < 0.1f)
        {
            currentTargetIndex++;
        }
    }

    public void StartAutoFollow()
    {
        if (pathGenerator == null)
        {
            Debug.LogError("AutoFollower: Cannot start auto-follow, PathGenerator not assigned!");
            return;
        }

        pathPoints = new List<Vector3>(pathGenerator.worldPathPoints);
        currentTargetIndex = 0;
        autoFollow = true;

        Debug.Log($"AutoFollower: Started auto-follow with {pathPoints.Count} points.");
    }

    public void StopAutoFollow()
    {
        autoFollow = false;
        Debug.Log("AutoFollower: Auto-follow stopped.");
    }
}