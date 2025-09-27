using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BreathingCycleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AutoFollower playerFollower;
    [SerializeField] private SpiritMovement spirit;
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
    [SerializeField] private string endSceneName = "BreathingEnd";

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private TorchVision torch;
    private float masteryProgress = 0f;
    private bool gameEnded = false;

    private void Start()
    {
        // Spawn torch prefab
        if (torchPrefab != null && torchParent != null)
        {
            GameObject torchInstance = Instantiate(torchPrefab, torchParent);
            torch = torchInstance.GetComponent<TorchVision>();
            if (torch == null)
                Debug.LogError("Torch prefab missing TorchVision component!");
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

        if (playerFollower == null || spirit == null)
        {
            Debug.LogError("Missing required references!");
            return;
        }

        if (progressBar != null) progressBar.value = 0f;
        if (statusText != null) statusText.text = "";

        spirit.gameObject.SetActive(false);
        StartCoroutine(BreathingCycle());
    }

    private IEnumerator BreathingCycle()
    {
        while (!gameEnded)
        {
            // ---------------- INHALE ----------------
            if (debugLogs) Debug.Log("Inhale");
            playerFollower.StartAutoFollow();
            spirit.gameObject.SetActive(false);
            if (statusText != null) statusText.text = "";
            yield return new WaitForSeconds(inhaleTime);

            // ---------------- HOLD ----------------
            if (debugLogs) Debug.Log("Hold");
            playerFollower.StopAutoFollow();
            spirit.gameObject.SetActive(true);
            if (statusText != null) statusText.text = "Point at the Spirit";

            float holdTimer = 0f;
            masteryProgress = 0f;

            while (holdTimer < holdTime && !gameEnded)
            {
                holdTimer += Time.deltaTime;

                if (torch != null && torch.IsPointingAt(spirit.transform))
                {
                    masteryProgress += Time.deltaTime;
                    if (progressBar != null)
                        progressBar.value = Mathf.Clamp01(masteryProgress / masteryTime);

                    if (masteryProgress >= masteryTime)
                    {
                        masteryProgress = masteryTime;
                        BreathingMastered();
                    }
                }
                else
                {
                    masteryProgress -= Time.deltaTime * 0.5f;
                    masteryProgress = Mathf.Max(0f, masteryProgress);
                    if (progressBar != null)
                        progressBar.value = Mathf.Clamp01(masteryProgress / masteryTime);
                }

                yield return null;
            }

            // ---------------- EXHALE ----------------
            if (debugLogs) Debug.Log("Exhale");
            spirit.gameObject.SetActive(false);
            playerFollower.StartAutoFollow();
            if (statusText != null) statusText.text = "";
            yield return new WaitForSeconds(exhaleTime);
        }
    }

    private void BreathingMastered()
    {
        if (gameEnded) return;

        gameEnded = true;
        if (statusText != null) statusText.text = "Breathing Mastered";
        Debug.Log("Breathing Mastered!");
        Invoke(nameof(EndGame), 2f);
    }

    private void EndGame()
    {
        if (!string.IsNullOrEmpty(endSceneName))
            SceneManager.LoadScene(endSceneName);
        else
            Debug.Log("Game would end here. No end scene set.");
    }
}