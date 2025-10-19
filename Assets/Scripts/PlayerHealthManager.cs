using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Player health bar that displays health using a set of sprites (e.g. 11 sprites).
/// Health decreases automatically based on the number of active spirits returned by a SpiritManager.
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to SpiritManager (if left empty it will try to find one in the scene).")]
    [SerializeField] private SpiritManager spiritManager;
    [Tooltip("UI Image used to display the health sprite.")]
    [SerializeField] private Image healthImage;

    [Header("Sprites")]
    [Tooltip("Ordered health sprites - index 0 = empty (dead), last index = full health.")]
    [SerializeField] private Sprite[] healthSprites; // expected length 11

    [Header("Health Settings")]
    [Tooltip("Player max health (treated as 1.0 for percentage calculations).")]
    [SerializeField] private float maxHealth = 1f;
    [Tooltip("Starting health (0..maxHealth).")]
    [SerializeField] private float startHealth = 1f;
    [Tooltip("Damage per second inflicted by *one* spirit.")]
    [SerializeField] private float damagePerSpiritPerSecond = 0.05f;
    [Tooltip("Optional passive regeneration per second (set to 0 to disable).")]
    [SerializeField] private float healthRegenPerSecond = 0f;
    [Tooltip("Minimum time between visual sprite updates (frames).")]
    [SerializeField] private float updateInterval = 0.05f;

    [Header("Debug / Options")]
    [SerializeField] private bool debugLogs = false;

    [Header("Events")]
    public UnityEvent onPlayerDeath;
    public UnityEvent<float> onHealthChanged; // passes current health (0..maxHealth)

    private float currentHealth;
    private float updateTimer = 0f;

    private void Awake()
    {
        if (spiritManager == null)
            spiritManager = FindAnyObjectByType<SpiritManager>();

        if (healthImage == null)
            Debug.LogWarning($"[{nameof(PlayerHealthBar)}] healthImage not assigned. Assign a UI Image to show the health sprite.");

        if (healthSprites == null || healthSprites.Length == 0)
            Debug.LogError($"[{nameof(PlayerHealthBar)}] No healthSprites assigned! Please assign 11 sprites in inspector.");

        currentHealth = Mathf.Clamp(startHealth, 0f, maxHealth);
        UpdateHealthVisualImmediate();
    }

    private void Update()
    {
        if (currentHealth <= 0f) return; // already dead; ignore further updates

        // compute damage from spirits
        int spiritCount = 0;
        if (spiritManager != null)
        {
            List<GameObject> active = spiritManager.GetActiveSpirits();
            if (active != null) spiritCount = active.Count;
        }

        // damage and regen
        float damageThisFrame = spiritCount * damagePerSpiritPerSecond * Time.deltaTime;
        float regenThisFrame = healthRegenPerSecond * Time.deltaTime;

        currentHealth = Mathf.Clamp(currentHealth - damageThisFrame + regenThisFrame, 0f, maxHealth);

        // update visual every updateInterval seconds to avoid excessive sprite churn
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateHealthVisual();
        }

        // broadcast health changed (optional)
        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            HandleDeath();
        }

        if (debugLogs && spiritCount > 0)
        {
            Debug.Log($"[PlayerHealthBar] spirits: {spiritCount}, damage this frame: {damageThisFrame:F4}, health: {currentHealth:F3}");
        }
    }

    /// <summary>
    /// Immediately update the health sprite without smoothing.
    /// </summary>
    private void UpdateHealthVisualImmediate()
    {
        if (healthSprites == null || healthSprites.Length == 0 || healthImage == null) return;

        float pct = Mathf.Clamp01(currentHealth / maxHealth);
        int index = Mathf.Clamp(Mathf.RoundToInt(pct * (healthSprites.Length - 1)), 0, healthSprites.Length - 1);
        healthImage.sprite = healthSprites[index];
    }

    /// <summary>
    /// Update the health sprite (called periodically).
    /// </summary>
    private void UpdateHealthVisual()
    {
        UpdateHealthVisualImmediate();
    }

    /// <summary>
    /// Public method to directly damage player (useful for other game systems).
    /// </summary>
    public void ApplyDamage(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        UpdateHealthVisualImmediate();
        onHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0f) HandleDeath();
    }

    /// <summary>
    /// Public method to heal player.
    /// </summary>
    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        UpdateHealthVisualImmediate();
        onHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Returns current health (0..maxHealth).
    /// </summary>
    public float GetHealth() => currentHealth;

    /// <summary>
    /// Called when player health reaches zero.
    /// </summary>
    private void HandleDeath()
    {
        if (debugLogs) Debug.Log("[PlayerHealthBar] Player died.");
        onPlayerDeath?.Invoke();
        // You may want to disable this script or trigger a respawn; keep as-is so the event can handle behavior.
    }

    #region Editor helpers (optional)
#if UNITY_EDITOR
    private void OnValidate()
    {
        // clamp sensible values in the editor
        maxHealth = Mathf.Max(0.0001f, maxHealth);
        startHealth = Mathf.Clamp(startHealth, 0f, maxHealth);
        updateInterval = Mathf.Max(0.01f, updateInterval);
    }
#endif
    #endregion
}