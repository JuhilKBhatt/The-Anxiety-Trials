using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the spirit moves around the circle.")]
    public float speed = 1f;

    [Tooltip("How much randomness to apply to the circular path (0 = perfect circle, 1 = chaotic).")]
    [Range(0f, 1f)]
    public float randomness = 0.2f;

    [Tooltip("Radius of the circular motion.")]
    public float radius = 2f;

    [Tooltip("How often (in seconds) the spirit might randomly reverse direction.")]
    public float reverseChanceInterval = 2f;

    [Tooltip("Chance (0-1) of reversing direction at each check.")]
    [Range(0f, 1f)]
    public float reverseChance = 0.3f;

    private float angle = 0f;
    private int direction = 1;
    private float nextReverseCheck = 0f;
    private Vector3 origin;

    void Start()
    {
        // Store original position as the circle center
        origin = transform.position;
        nextReverseCheck = Time.time + reverseChanceInterval;
    }

    void Update()
    {
        // Randomly decide whether to reverse direction
        if (Time.time >= nextReverseCheck)
        {
            nextReverseCheck = Time.time + reverseChanceInterval;
            if (Random.value < reverseChance)
            {
                direction *= -1; // Reverse!
            }
        }

        // Advance the angle
        angle += direction * speed * Time.deltaTime;

        // Add random "noise" to the path
        float randomOffset = Mathf.Sin(Time.time * speed * 2f) * randomness;

        // Compute new position
        float x = Mathf.Cos(angle + randomOffset) * radius;
        float y = Mathf.Sin(angle - randomOffset) * radius;

        // Apply movement
        transform.position = origin + new Vector3(x, y, 0f);
    }
}