using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BreathingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image progressBar;

    [Header("References")]
    [SerializeField] private BreathingCycleManager breathingManager;

    private float inhaleTime;
    private float holdTime;
    private float exhaleTime;

    private void Start()
    {
        // 🔍 Find manager if not assigned
        if (breathingManager == null)
            breathingManager = FindAnyObjectByType<BreathingCycleManager>();

        if (breathingManager == null)
        {
            Debug.LogError("BreathingUI: No BreathingCycleManager found in scene!");
            return;
        }

        // Pull breathing timings from manager
        inhaleTime = breathingManager.InhaleTime;
        holdTime = breathingManager.HoldTime;
        exhaleTime = breathingManager.ExhaleTime;

        StartCoroutine(BreathingRoutine());
    }

    private IEnumerator BreathingRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(DoPhase("Inhale...", inhaleTime));
            yield return StartCoroutine(DoPhase("Hold...", holdTime));
            yield return StartCoroutine(DoPhase("Exhale...", exhaleTime));
        }
    }

    private IEnumerator DoPhase(string instruction, float duration)
    {
        instructionText.text = instruction;

        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();

            if (progressBar != null)
            {
                progressBar.fillAmount = 1f - (timer / duration);
            }

            yield return null;
        }

        timerText.text = "0";
        if (progressBar != null) progressBar.fillAmount = 1f;
    }
}