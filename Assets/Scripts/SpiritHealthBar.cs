using UnityEngine;

public class SpiritHealthBar : MonoBehaviour
{
    [Header("Sprites for each health stage (full to empty)")]
    [SerializeField] private Sprite[] healthSprites;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;   // Health bar sprite
    [SerializeField] private SpriteRenderer spiritRenderer;   // Spirit body sprite

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Opacity Settings")]
    [SerializeField, Range(0f, 1f)] private float minOpacity = 0.3f; // fully transparent at 0 HP
    [SerializeField, Range(0f, 1f)] private float maxOpacity = 1f;   // fully opaque at full HP

    public bool IsDead => currentHealth <= 0f;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Try auto-assign spirit body if not set
        if (spiritRenderer == null)
            spiritRenderer = GetComponentInParent<SpriteRenderer>();

        currentHealth = maxHealth;
        UpdateHealthSprite();
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateHealthSprite();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthSprite();
    }

    private void UpdateHealthSprite()
    {
        if (healthSprites == null || healthSprites.Length == 0 || spriteRenderer == null)
            return;

        float healthPercent = currentHealth / maxHealth;
        int index = Mathf.FloorToInt(healthPercent * (healthSprites.Length - 1));
        index = Mathf.Clamp(index, 0, healthSprites.Length - 1);

        spriteRenderer.sprite = healthSprites[index];

        // Update spirit opacity
        if (spiritRenderer != null)
        {
            float alpha = Mathf.Lerp(minOpacity, maxOpacity, healthPercent);
            Color c = spiritRenderer.color;
            c.a = alpha;
            spiritRenderer.color = c;
        }
    }
}