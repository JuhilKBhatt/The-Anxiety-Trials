using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 1f;
    [Range(0f, 1f)] public float randomness = 0.2f;
    public float radius = 2f;
    public float reverseChanceInterval = 2f;
    [Range(0f, 1f)] public float reverseChance = 0.3f;

    public Transform centerTarget; // The player to orbit

    private float angle = 0f;
    private int direction = 1;
    private float nextReverseCheck = 0f;

    void Start()
    {
        nextReverseCheck = Time.time + reverseChanceInterval;
    }

    void Update()
    {
        if (centerTarget == null) return;

        // Randomly reverse
        if (Time.time >= nextReverseCheck)
        {
            nextReverseCheck = Time.time + reverseChanceInterval;
            if (Random.value < reverseChance)
                direction *= -1;
        }

        // Advance angle
        angle += direction * speed * Time.deltaTime;

        // Add random "noise"
        float randomOffset = Mathf.Sin(Time.time * speed * 2f) * randomness;

        // Compute position around the target
        float x = Mathf.Cos(angle + randomOffset) * radius;
        float y = Mathf.Sin(angle - randomOffset) * radius;

        transform.position = centerTarget.position + new Vector3(x, y, 0f);
    }
}