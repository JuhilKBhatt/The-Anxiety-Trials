using System.Collections;
using UnityEngine;

public class BreathingCycleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AutoFollower playerFollower;   // Auto-walk script
    [SerializeField] private SpiritMovement spirit;         // Spirit to target
    [SerializeField] private GameObject torchPrefab;        // Torch prefab (assigned in inspector)
    [SerializeField] private Transform torchParent;         // Where to attach the torch (e.g., player's hand or player root)

    [Header("Breathing Timings (seconds)")]
    [SerializeField] private float inhaleTime = 4f;
    [SerializeField] private float holdTime = 7f;
    [SerializeField] private float exhaleTime = 8f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private TorchVision torch;

    private void Start()
    {
        // 1️⃣ Spawn or find torch at runtime
        if (torchPrefab != null && torchParent != null)
        {
            GameObject torchInstance = Instantiate(torchPrefab, torchParent);
            torch = torchInstance.GetComponent<TorchVision>();

            if (torch == null)
                Debug.LogError("BreathingCycleManager: Torch prefab does not have a TorchVision component!");
        }
        else
        {
            // If it's already in the scene (not spawned), try to find it
            torch = FindAnyObjectByType<TorchVision>();
            if (torch == null)
            {
                Debug.LogError("BreathingCycleManager: No TorchVision found or prefab not assigned!");
                return;
            }
        }

        if (playerFollower == null || spirit == null)
        {
            Debug.LogError("BreathingCycleManager: Missing required references!");
            return;
        }

        // Ensure spirit is hidden at start
        spirit.gameObject.SetActive(false);

        // Start the breathing cycle
        StartCoroutine(BreathingCycle());
    }

    private IEnumerator BreathingCycle()
    {
        while (true)
        {
            // ---------------- INHALE ----------------
            playerFollower.StartAutoFollow(); 
            spirit.gameObject.SetActive(false);

            if (debugLogs) Debug.Log("🌬 Inhale: Player walking for " + inhaleTime + "s");
            yield return new WaitForSeconds(inhaleTime);

            // ---------------- HOLD ----------------
            playerFollower.StopAutoFollow(); 
            spirit.gameObject.SetActive(true);

            if (debugLogs) Debug.Log("⏸ Hold: Aim torch at spirit for " + holdTime + "s");

            float holdTimer = 0f;
            while (holdTimer < holdTime)
            {
                holdTimer += Time.deltaTime;

                // ✅ Example detection placeholder
                // if (torch.IsPointingAt(spirit.transform))
                // {
                //     Debug.Log("🔦 Torch is on the spirit!");
                // }

                yield return null;
            }

            // ---------------- EXHALE ----------------
            spirit.gameObject.SetActive(false);
            playerFollower.StartAutoFollow();

            if (debugLogs) Debug.Log("💨 Exhale: Player walking for " + exhaleTime + "s");
            yield return new WaitForSeconds(exhaleTime);
        }
    }
}