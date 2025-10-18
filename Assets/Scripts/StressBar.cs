using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    // A static reference to the single instance of this class
    public static StressBar Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("The UI Image component that will display the stress sprite.")]
    [SerializeField] private Image stressBarImage;

    [Header("Stress Sprites")]
    [Tooltip("The array of sprites representing the stress level. Element 0 should be the calmest (full bar), and the last element should be the most stressed (empty bar).")]
    [SerializeField] private Sprite[] stressSprites;

    // The current stress level, represented as an index for the stressSprites array.
    private int currentStressLevel;

    private void Awake()
    {
        // --- Singleton Pattern Implementation ---
        // If an instance already exists and it's not this one, destroy this one.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Otherwise, set this as the single instance.
            Instance = this;
            // Optional: if this needs to persist between scenes, uncomment the next line
            // DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // 🛡️ Safety check to ensure everything is set up in the Inspector.
        if (stressSprites == null || stressSprites.Length == 0)
        {
            Debug.LogError("StressBar: The 'Stress Sprites' array is not set up. Please assign sprites in the Inspector.");
            enabled = false; // Disable the script
            return;
        }
        if (stressBarImage == null)
        {
            Debug.LogError("StressBar: The 'Stress Bar Image' is not assigned. Please drag the UI Image component to this slot in the Inspector.");
            enabled = false; // Disable the script
            return;
        }

        // Initialize stress to a neutral, middle state.
        currentStressLevel = stressSprites.Length / 2;
        UpdateStressVisual();
    }

    /// <summary>
    /// Increases the stress level by one step (moves towards the 'empty' sprite).
    /// </summary>
    public void IncreaseStress()
    {
        // Increase the level, but don't go past the end of the array.
        currentStressLevel = Mathf.Clamp(currentStressLevel + 1, 0, stressSprites.Length - 1);
        UpdateStressVisual();
    }

    /// <summary>
    /// Decreases the stress level by one step (moves towards the 'full' sprite).
    /// </summary>
    public void DecreaseStress()
    {
        // Decrease the level, but don't go below zero.
        currentStressLevel = Mathf.Clamp(currentStressLevel - 1, 0, stressSprites.Length - 1);
        UpdateStressVisual();
    }

    /// <summary>
    /// Updates the displayed sprite based on the current stress level.
    /// </summary>
    private void UpdateStressVisual()
    {
        // Set the sprite of the Image component to the one corresponding to the current stress level.
        stressBarImage.sprite = stressSprites[currentStressLevel];
    }
}