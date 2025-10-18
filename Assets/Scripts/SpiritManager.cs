using UnityEngine;

public class SpiritManager : MonoBehaviour
{
    [Header("Spirit Settings")]
    [SerializeField] private GameObject spiritPrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Transform orbitTarget; // Usually the player
    [SerializeField] private float maxHealth = 100f;

    private GameObject currentSpirit;
    private SpiritHealthBar currentHealthBar;
    private float currentHealth;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private void Start()
    {
        SpawnSpirit();
    }

    public void SpawnSpirit()
    {
        if (spiritPrefab == null)
        {
            Debug.LogError("SpiritManager: No Spirit prefab assigned!");
            return;
        }

        if (currentSpirit != null)
            Destroy(currentSpirit);

        currentSpirit = Instantiate(spiritPrefab, spawnParent);

        // Set orbit center
        SpiritMovement movement = currentSpirit.GetComponent<SpiritMovement>();
        if (movement != null)
            movement.centerTarget = orbitTarget;

        // Grab health bar (must be child of spirit prefab)
        currentHealthBar = currentSpirit.GetComponentInChildren<SpiritHealthBar>();

        currentHealth = maxHealth;
        if (currentHealthBar != null)
            currentHealthBar.UpdateHealthBar(1f);

        if (debugLogs)
            Debug.Log("Spawned new Spirit.");
    }

    public bool DamageSpirit(float amount)
    {
        if (currentSpirit == null)
            return false;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        // Update visual health bar
        if (currentHealthBar != null)
            currentHealthBar.UpdateHealthBar(currentHealth / maxHealth);

        if (debugLogs)
            Debug.Log($"Spirit health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            if (debugLogs)
                Debug.Log("Spirit died. Respawning...");
            SpawnSpirit();
            return true;
        }

        return false;
    }

    public GameObject GetSpirit() => currentSpirit;
    public float GetHealth() => currentHealth;
}