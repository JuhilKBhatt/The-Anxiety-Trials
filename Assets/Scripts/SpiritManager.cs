using UnityEngine;

public class SpiritManager : MonoBehaviour
{
    [Header("Spirit Settings")]
    [SerializeField] private GameObject spiritPrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Transform orbitTarget; // Usually the player
    [SerializeField] private float maxHealth = 1f;

    private GameObject currentSpirit;
    private float currentHealth;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private void Start()
    {
        SpawnSpirit();
    }

    /// <summary>
    /// Spawns a new Spirit prefab.
    /// </summary>
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
        SpiritMovement movement = currentSpirit.GetComponent<SpiritMovement>();
        if (movement != null)
            movement.centerTarget = orbitTarget;

        currentHealth = maxHealth;

        if (debugLogs)
            Debug.Log("Spawned new Spirit.");
    }

    /// <summary>
    /// Reduces the spirit's health by a given amount. Returns true if the spirit died.
    /// </summary>
    public bool DamageSpirit(float amount)
    {
        if (currentSpirit == null)
            return false;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (debugLogs)
            Debug.Log($"Spirit health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            // Spirit died → respawn
            if (debugLogs)
                Debug.Log("Spirit died. Respawning...");

            SpawnSpirit();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the current Spirit GameObject.
    /// </summary>
    public GameObject GetSpirit()
    {
        return currentSpirit;
    }

    /// <summary>
    /// Returns the current health of the Spirit.
    /// </summary>
    public float GetHealth()
    {
        return currentHealth;
    }
}