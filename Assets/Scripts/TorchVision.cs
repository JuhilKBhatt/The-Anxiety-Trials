using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class TorchVision : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private string[] detectableTags;

    [Header("Mouse Tracking")]
    public bool TrackMouse = true;

    private Light2D torchLight;

    private void Awake()
    {
        torchLight = GetComponent<Light2D>();
    }

    private void Update()
    {
        if (TrackMouse)
            RotateTorchTowardsMouse();

        // Optional: detect objects in light cone every frame
        DetectObjectsInLightCone();
    }

    private void RotateTorchTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mousePos - transform.position;
        dir.z = 0f;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // Adjust for Up direction
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
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
            float angle = Vector2.Angle(transform.up, dirToTarget);

            if (angle <= coneAngle)
            {
                // Currently detected object is inside cone
                // You can add visual effects, log, or pass info to other scripts
            }
        }
    }

    private bool HasDetectableTag(GameObject obj)
    {
        foreach (string tag in detectableTags)
        {
            if (obj.CompareTag(tag))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true if the torch is pointing at the given Transform
    /// within its light cone.
    /// </summary>
    public bool IsPointingAt(Transform target)
    {
        if (target == null) return false;

        float radius = torchLight.pointLightOuterRadius;
        float coneAngle = torchLight.pointLightOuterAngle * 0.5f;

        Vector2 dirToTarget = (target.position - transform.position).normalized;
        float angle = Vector2.Angle(transform.up, dirToTarget);

        return Vector2.Distance(transform.position, target.position) <= radius && angle <= coneAngle;
    }

    /// <summary>
    /// Returns true if the torch is pointing at any object with a detectable tag.
    /// </summary>
    public bool IsPointingAtTag()
    {
        float radius = torchLight.pointLightOuterRadius;
        float coneAngle = torchLight.pointLightOuterAngle * 0.5f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, detectionMask);
        foreach (Collider2D hit in hits)
        {
            if (!HasDetectableTag(hit.gameObject))
                continue;

            Vector2 dirToTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.up, dirToTarget);

            if (angle <= coneAngle)
                return true;
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

        Vector3 facingDir = transform.up * radius;
        Quaternion leftRot = Quaternion.AngleAxis(-coneAngle, Vector3.forward);
        Quaternion rightRot = Quaternion.AngleAxis(coneAngle, Vector3.forward);

        Vector3 leftDir = leftRot * facingDir;
        Vector3 rightDir = rightRot * facingDir;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftDir);
        Gizmos.DrawLine(transform.position, transform.position + rightDir);
    }

    /// <summary>
    /// Returns a list of all objects with detectable tags inside the torch cone.
    /// </summary>
    public List<GameObject> GetObjectsInCone()
    {
        List<GameObject> detected = new List<GameObject>();

        float radius = torchLight.pointLightOuterRadius;
        float coneAngle = torchLight.pointLightOuterAngle * 0.5f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, detectionMask);
        foreach (Collider2D hit in hits)
        {
            if (!HasDetectableTag(hit.gameObject))
                continue;

            Vector2 dirToTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.up, dirToTarget);

            if (angle <= coneAngle)
                detected.Add(hit.gameObject);
        }

        return detected;
    }
}