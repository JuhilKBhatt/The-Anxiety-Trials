using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StressValue stressValue;
    [SerializeField] private Image stressBarImage;
    [SerializeField] private Sprite[] stressSprites;

    private void Start()
    {
        if (stressValue == null)
        {
            Debug.LogError("StressBar: Missing StressValue ScriptableObject reference!");
            enabled = false;
            return;
        }
        if (stressSprites == null || stressSprites.Length == 0)
        {
            Debug.LogError("StressBar: No stress sprites assigned!");
            enabled = false;
            return;
        }
        if (stressBarImage == null)
        {
            Debug.LogError("StressBar: No UI Image assigned!");
            enabled = false;
            return;
        }

        UpdateStressVisual();
    }

    private void Update()
    {
        UpdateStressVisual();
    }

    private void UpdateStressVisual()
    {
        // Convert stress value (0–1) to sprite index
        int spriteIndex = Mathf.RoundToInt(stressValue.value * (stressSprites.Length - 1));
        stressBarImage.sprite = stressSprites[spriteIndex];
    }
}