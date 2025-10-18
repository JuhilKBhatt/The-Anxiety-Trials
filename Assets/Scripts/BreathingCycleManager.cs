using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BreathingCycleManager : MonoBehaviour
{
    [SerializeField] private SpiritManager spiritManager;

    [Header("References")]
    [SerializeField] private AutoFollower playerFollower;
    [SerializeField] private GameObject torchPrefab;
    [SerializeField] private Transform torchParent;

    [Header("Breathing Timings (seconds)")]
    [SerializeField] private float inhaleTime = 4f;
    [SerializeField] private float holdTime = 7f;
    [SerializeField] private float exhaleTime = 8f;
    public float InhaleTime => inhaleTime;
    public float HoldTime => holdTime;
    public float ExhaleTime => exhaleTime;

    [Header("Breathing Mastery UI")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private float masteryTime = 10f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private TorchVision torch;
    private float masteryProgress = 0f;
    private bool gameEnded = false;
    private Coroutine breathingRoutine;

    private void Start()
    {
        // Spawn or find Torch
        if (torchPrefab != null && torchParent != null)
        {
            GameObject torchInstance = Instantiate(torchPrefab, torchParent);
            torch = torchInstance.GetComponent<TorchVision>();
            if (torch == null)
                Debug.LogError("Torch prefab is missing a TorchVision component!");
        }
        else
        {
            torch = FindAnyObjectByType<TorchVision>();
            if (torch == null)
            {
                Debug.LogError("No TorchVision found or prefab not assigned!");
                return;
            }
        }

        if (playerFollower == null)
        {
            Debug.LogError("Missing AutoFollower reference!");
            return;
        }

        // Initialize UI
        if (progressBar != null) progressBar.value = 0f;
        if (statusText != null) statusText.text = "";

        // Ensure player keeps walking
        playerFollower.StartAutoFollow();

        breathingRoutine = StartCoroutine(BreathingCycle());
    }

    private IEnumerator BreathingCycle()
    {
        while (!gameEnded)
        {
            // --- INHALE ---
            if (debugLogs) Debug.Log("Inhale");
            if (statusText != null) statusText.text = "Inhale...";
            yield return StartCoroutine(PhaseRoutine(inhaleTime));

            if (gameEnded) yield break;

            // --- HOLD ---
            if (debugLogs) Debug.Log("Hold");
            if (statusText != null) statusText.text = "Hold... focus your torch.";
            yield return StartCoroutine(PhaseRoutine(holdTime));

            if (gameEnded) yield break;

            // --- EXHALE ---
            if (debugLogs) Debug.Log("Exhale");
            if (statusText != null) statusText.text = "Exhale...";
            yield return StartCoroutine(PhaseRoutine(exhaleTime));
        }
    }

    /// <summary>
    /// Handles torching spirits and mastery progress during any breathing phase.
    /// </summary>
    private IEnumerator PhaseRoutine(float duration)
    {
        float timer = 0f;

        while (timer < duration && !gameEnded)
        {
            timer += Time.deltaTime;

            // Torch spirits every frame
            List<GameObject> detectedSpirits = torch.GetObjectsInCone();
            if (detectedSpirits.Count > 0)
            {
                foreach (var spirit in detectedSpirits)
                {
                    spiritManager.DamageSpirit(spirit, 0.5f * Time.deltaTime);
                }
                masteryProgress += Time.deltaTime;
            }
            else
            {
                masteryProgress -= Time.deltaTime * 0.25f;
            }

            masteryProgress = Mathf.Clamp(masteryProgress, 0f, masteryTime);

            if (progressBar != null)
                progressBar.value = Mathf.Clamp01(masteryProgress / masteryTime);

            if (masteryProgress >= masteryTime)
            {
                BreathingMastered();
                yield break;
            }

            yield return null;
        }
    }

    private void BreathingMastered()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (breathingRoutine != null)
            StopCoroutine(breathingRoutine);

        playerFollower.StopAutoFollow();

        if (statusText != null)
            statusText.text = "Breathing Mastered - End of Game";

        Debug.Log("Breathing Mastered! Game stopped.");
    }
}