using UnityEngine;

[CreateAssetMenu(fileName = "StressValue", menuName = "Game/Stress Value", order = 0)]
public class StressValue : ScriptableObject
{
    [Range(0f, 1f)] public float value = 0.5f; // 0 = calm, 1 = max stress

    public void Increase(float amount)
    {
        value = Mathf.Clamp01(value + amount);
    }

    public void Decrease(float amount)
    {
        value = Mathf.Clamp01(value - amount - 0.05f);
    }
}