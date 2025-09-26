using System.Collections;
using UnityEngine;

public class BreathingCycleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AutoFollower playerFollower;  // Auto-walk script
    [SerializeField] private SpiritMovement spirit;        // Spirit to target
    [SerializeField] private TorchVision torch;           // Player's torch script

    [Header("Breathing Timings (seconds)")]
    [SerializeField] private float inhaleTime = 4f;
    [SerializeField] private float holdTime = 7f;
    [SerializeField] private float exhaleTime = 8f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private void Start()
    {
        if (playerFollower == null || spirit == null || torch == null)
        {
            Debug.LogError("BreathingCycleManager: References not set!");
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
            playerFollower.StartAutoFollow(); // Player walks
            spirit.gameObject.SetActive(false);

            if (debugLogs) Debug.Log("Inhale: Player walking for " + inhaleTime + "s");
            yield return new WaitForSeconds(inhaleTime);

            // ---------------- HOLD ----------------
            playerFollower.StopAutoFollow(); // Stop walking
            spirit.gameObject.SetActive(true);  // Spirit appears

            if (debugLogs) Debug.Log("Hold: Player aims torch at spirit for " + holdTime + "s");

            float holdTimer = 0f;
            while (holdTimer < holdTime)
            {
                holdTimer += Time.deltaTime;

                // TODO: Check if torch is pointing at spirit
                // Example: 
                // if (torch.IsPointingAt(spirit.transform)) { Debug.Log("Spirit targeted!"); }

                yield return null;
            }

            // ---------------- EXHALE ----------------
            spirit.gameObject.SetActive(false);
            playerFollower.StartAutoFollow(); // Continue walking

            if (debugLogs) Debug.Log("Exhale: Player walking for " + exhaleTime + "s");
            yield return new WaitForSeconds(exhaleTime);
        }
    }
}