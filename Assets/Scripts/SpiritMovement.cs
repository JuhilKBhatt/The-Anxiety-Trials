using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 0.25f;
    [Range(0f, 1f)] public float randomness = 0.6f;
    public float radius = 5f;
    public float reverseChanceInterval = 1.5f;
    [Range(0f, 1f)] public float reverseChance = 0.85f;

    public Transform centerTarget; // The player to orbit

    private float angle = 0f;
    private int direction = 1;
    private float nextReverseCheck = 0f;

    void Start()
    {
        nextReverseCheck = Time.time + reverseChanceInterval;
        angle = Random.Range(0f, 2f * Mathf.PI); // random starting position
        radius = Random.Range(4f, 6f); // optional spread radius
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