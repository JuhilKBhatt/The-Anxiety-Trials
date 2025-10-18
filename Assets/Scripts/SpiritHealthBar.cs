using UnityEngine;

public class SpiritHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer healthBarRenderer;
    [SerializeField] private Sprite[] healthBarSprites; // 6 sprites: 0–5 stages
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);

    private Transform spiritTransform;
    public float CurrentHealth { get; private set; } = 1f;

    private void Start()
    {
        spiritTransform = transform.parent; // Assuming the health bar is child of the spirit
    }

    private void LateUpdate()
    {
        if (spiritTransform != null)
            transform.position = spiritTransform.position + offset;
    }

    public void UpdateHealthBar(float percent)
    {
        CurrentHealth = Mathf.Clamp01(percent);

        if (healthBarSprites.Length == 0 || healthBarRenderer == null)
            return;

        int index = Mathf.Clamp(
            Mathf.RoundToInt(CurrentHealth * (healthBarSprites.Length - 1)),
            0,
            healthBarSprites.Length - 1
        );

        healthBarRenderer.sprite = healthBarSprites[index];
    }
}