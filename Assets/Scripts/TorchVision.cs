using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class TorchVision : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private string[] detectableTags;

    [Header("Torch Direction")]
    [Tooltip("Which direction the torch is pointing. Most 2D spot lights point 'Up'.")]
    [SerializeField] private Direction torchDirection = Direction.Up;

    private Light2D torchLight;

    private enum Direction { Up, Right, Down, Left }

    private void Awake()
    {
        torchLight = GetComponent<Light2D>();
    }

    private void Update()
    {
        DetectObjectsInLightCone();
    }

    private void DetectObjectsInLightCone()
    {
        float radius = torchLight.pointLightOuterRadius; 
        float coneAngle = torchLight.pointLightOuterAngle * 0.5f; 

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, detectionMask);

        foreach (Collider2D hit in hits)
        {
            if (!HasDetectableTag(hit.gameObject))
                continue;

            Vector2 dirToTarget = (hit.transform.position - transform.position).normalized;
            Vector2 facingDir = GetFacingDirection();

            float angle = Vector2.Angle(facingDir, dirToTarget);
            if (angle <= coneAngle)
            {
                Debug.Log($"🔥 Detected [{hit.gameObject.tag}] in torchlight: {hit.gameObject.name}");
            }
        }
    }

    private Vector2 GetFacingDirection()
    {
        switch (torchDirection)
        {
            case Direction.Up: return transform.up;
            case Direction.Down: return -transform.up;
            case Direction.Left: return -transform.right;
            case Direction.Right: return transform.right;
            default: return transform.up;
        }
    }

    private bool HasDetectableTag(GameObject obj)
    {
        foreach (string tag in detectableTags)
        {
            if (obj.CompareTag(tag)) return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (torchLight == null)
            torchLight = GetComponent<Light2D>();

        float radius = torchLight.pointLightOuterRadius;
        float coneAngle = torchLight.pointLightOuterAngle * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Visualize cone
        Vector3 facingDir = GetFacingDirection() * radius;
        Quaternion leftRot = Quaternion.AngleAxis(-coneAngle, Vector3.forward);
        Quaternion rightRot = Quaternion.AngleAxis(coneAngle, Vector3.forward);

        Vector3 leftDir = leftRot * facingDir;
        Vector3 rightDir = rightRot * facingDir;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftDir);
        Gizmos.DrawLine(transform.position, transform.position + rightDir);
    }
}