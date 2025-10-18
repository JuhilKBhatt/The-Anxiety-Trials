using System.Collections.Generic;
using UnityEngine;

public class SpiritManager : MonoBehaviour
{
    [Header("Spirit Settings")]
    [SerializeField] private GameObject spiritPrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Transform orbitTarget; // Usually the player
    [SerializeField] private int maxSpirits = 10;

    [Header("Stress Integration")]
    [SerializeField] private StressValue globalStressValue; // ScriptableObject

    [Header("Spawn Rate (seconds)")]
    [SerializeField] private float minSpawnRate = 5f; // at low stress
    [SerializeField] private float maxSpawnRate = 1f; // at high stress

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private List<GameObject> activeSpirits = new List<GameObject>();
    private float spawnTimer = 0f;

    private void Update()
    {
        if (globalStressValue == null || spiritPrefab == null || orbitTarget == null)
            return;

        spawnTimer -= Time.deltaTime;

        float currentSpawnRate = Mathf.Lerp(minSpawnRate, maxSpawnRate, globalStressValue.value);

        if (spawnTimer <= 0f && activeSpirits.Count < maxSpirits)
        {
            SpawnSpirit();
            spawnTimer = currentSpawnRate;
        }
    }

    public void SpawnSpirit()
    {
        GameObject newSpirit = Instantiate(spiritPrefab, spawnParent);

        // Set orbit target
        SpiritMovement movement = newSpirit.GetComponent<SpiritMovement>();
        if (movement != null)
            movement.centerTarget = orbitTarget;

        // Initialize health
        SpiritHealthBar healthBar = newSpirit.GetComponentInChildren<SpiritHealthBar>();
        if (healthBar != null)
            healthBar.UpdateHealthBar(1f);

        activeSpirits.Add(newSpirit);

        if (debugLogs)
            Debug.Log($"Spawned Spirit. Total spirits: {activeSpirits.Count}");
    }

    public void DamageSpirit(GameObject spirit, float amount)
    {
        if (spirit == null || !activeSpirits.Contains(spirit))
            return;

        SpiritHealthBar healthBar = spirit.GetComponentInChildren<SpiritHealthBar>();
        if (healthBar != null)
        {
            float currentHealth = healthBar.CurrentHealth; // We'll add this property in SpiritHealthBar
            currentHealth -= amount;
            healthBar.UpdateHealthBar(currentHealth);
            if (currentHealth <= 0f)
            {
                activeSpirits.Remove(spirit);
                Destroy(spirit);
                if (debugLogs) Debug.Log("Spirit destroyed due to 0 health");
            }
        }
    }

    public List<GameObject> GetActiveSpirits() => activeSpirits;
}