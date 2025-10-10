using UnityEngine;

public class SpiritHealthBar : MonoBehaviour
{
    [Header("Sprites for each health stage (ordered from full to empty)")]
    [SerializeField] private Sprite[] healthSprites;

    [Header("Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private void Awake()
    {
        // Auto-find SpriteRenderer if not set manually
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

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

        // Calculate which sprite to use
        float healthPercent = currentHealth / maxHealth;
        int index = Mathf.FloorToInt(healthPercent * (healthSprites.Length - 1));

        // Invert if your sprite order is reversed (optional)
        index = Mathf.Clamp(index, 0, healthSprites.Length - 1);

        spriteRenderer.sprite = healthSprites[index];
    }
}