using UnityEngine;

public class SpiritHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpiritManager spiritManager;
    [SerializeField] private SpriteRenderer healthBarRenderer;
    [SerializeField] private Sprite[] healthBarSprites; // 6 sprites: 0–5 stages
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);

    private Transform spiritTransform;
    private float lastPercent = 1f;

    private void Start()
    {
        if (spiritManager == null)
            spiritManager = FindFirstObjectByType<SpiritManager>();

        if (healthBarRenderer == null)
            healthBarRenderer = GetComponent<SpriteRenderer>();

        if (spiritManager != null && spiritManager.GetSpirit() != null)
            spiritTransform = spiritManager.GetSpirit().transform;
    }

    private void LateUpdate()
    {
        if (spiritManager == null || spiritManager.GetSpirit() == null)
            return;

        if (spiritTransform == null)
            spiritTransform = spiritManager.GetSpirit().transform;

        // Follow the spirit
        transform.position = spiritTransform.position + offset;
    }

    /// <summary>
    /// Called by SpiritManager to update the health bar (0–1).
    /// </summary>
    public void UpdateHealthBar(float percent)
    {
        lastPercent = Mathf.Clamp01(percent);

        if (healthBarSprites.Length == 0)
            return;

        int index = Mathf.Clamp(
            Mathf.RoundToInt(lastPercent * (healthBarSprites.Length - 1)),
            0,
            healthBarSprites.Length - 1
        );

        healthBarRenderer.sprite = healthBarSprites[index];
    }
}