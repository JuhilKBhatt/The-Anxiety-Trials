using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreathingCycleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AutoFollower playerFollower;
    [SerializeField] private SpiritMovement spirit;
    [SerializeField] private GameObject torchPrefab;
    [SerializeField] private Transform torchParent;
    [SerializeField] private GameObject spiritPrefab;  // 👈 new: assign your Spirit prefab here
    [SerializeField] private Transform spiritSpawnParent; // optional, if you want a parent for spirits

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
        breathingRoutine = StartCoroutine(BreathingCycle());
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
            yield return WaitOrBreak(inhaleTime);

            if (gameEnded) yield break;

            // ---------------- HOLD ----------------
            if (debugLogs) Debug.Log("Hold");
            playerFollower.StopAutoFollow();
            spirit.gameObject.SetActive(true);
            if (statusText != null) statusText.text = "Point at the Spirit";

            float holdTimer = 0f;
            while (holdTimer < holdTime && !gameEnded)
            {
                holdTimer += Time.deltaTime;

                if (torch != null && torch.IsPointingAt(spirit.transform))
                {
                    SpiritHealthBar spiritHealth = spirit.GetComponentInChildren<SpiritHealthBar>();
                    if (spiritHealth != null)
                    {
                        spiritHealth.TakeDamage(0.15f);

                        if (spiritHealth.IsDead)
                        {
                            Destroy(spirit.gameObject);
                            yield return new WaitForSeconds(0.5f);
                            SpawnNewSpirit();
                            yield break; // Restart cycle with new spirit
                        }
                    }

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
                    masteryProgress -= Time.deltaTime * 0.25f;
                    masteryProgress = Mathf.Max(0f, masteryProgress);
                    if (progressBar != null)
                        progressBar.value = Mathf.Clamp01(masteryProgress / masteryTime);
                }

                yield return null;
            }

            if (gameEnded) yield break;

            // ---------------- EXHALE ----------------
            if (debugLogs) Debug.Log("Exhale");
            spirit.gameObject.SetActive(false);
            playerFollower.StartAutoFollow();
            if (statusText != null) statusText.text = "";
            yield return WaitOrBreak(exhaleTime);
        }
    }

    private void SpawnNewSpirit()
    {
        if (spiritPrefab == null)
        {
            Debug.LogError("Spirit prefab not assigned!");
            return;
        }

        // Instantiate new Spirit prefab
        GameObject newSpiritObj = Instantiate(spiritPrefab, spiritSpawnParent);
        spirit = newSpiritObj.GetComponent<SpiritMovement>();

        if (debugLogs) Debug.Log("Spawned a new Spirit prefab.");
    }

    private void BreathingMastered()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (breathingRoutine != null)
            StopCoroutine(breathingRoutine);

        playerFollower.StopAutoFollow();
        spirit.gameObject.SetActive(false);

        if (statusText != null)
            statusText.text = "Breathing Mastered - End of Game ";

        Debug.Log("Breathing Mastered! Game stopped.");
    }

    private IEnumerator WaitOrBreak(float seconds)
    {
        float t = 0f;
        while (t < seconds && !gameEnded)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }
}