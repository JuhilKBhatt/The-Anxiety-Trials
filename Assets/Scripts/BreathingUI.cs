using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum BreathingPhaseType
{
    Tap,
    Hold
}

[System.Serializable]
public class BreathingPhase
{
    public string instruction;
    public float duration = 4f;
    public KeyCode keyToPress;
    public Sprite keyUpSprite;
    public Sprite keyDownSprite;
    public BreathingPhaseType phaseType = BreathingPhaseType.Hold;
}

public class BreathingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image progressBar;
    [SerializeField] private Image keySpriteImage;
    [SerializeField] private Image tickDisplayImage;
    [SerializeField] private Sprite correctTickSprite;

    [Header("Breathing Phases")]
    [SerializeField] private BreathingPhase[] breathingPhases;
    [SerializeField] private float keyAnimationSpeed = 0.3f;

    [Header("Stress Integration")]
    [Tooltip("Reference to the shared StressValue ScriptableObject.")]
    [SerializeField] private StressValue stressValue;
    [Tooltip("How often (in seconds) to increase or decrease stress during HOLD phases.")]
    [SerializeField] private float stressTickRate = 0.5f;
    [Tooltip("Amount of stress to change per tick (for Hold) or per phase (for Tap).")]
    [SerializeField] private float stressChangeAmount = 0.05f;

    private Coroutine _keyAnimationCoroutine;

    private void Start()
    {
        if (breathingPhases == null || breathingPhases.Length == 0)
        {
            Debug.LogError("BreathingUI: No breathing phases have been set up!");
            return;
        }

        if (stressValue == null)
        {
            Debug.LogError("BreathingUI: Missing StressValue ScriptableObject reference!");
            return;
        }

        keySpriteImage.gameObject.SetActive(false);
        tickDisplayImage.gameObject.SetActive(false);

        StartCoroutine(BreathingRoutine());
    }

    private IEnumerator BreathingRoutine()
    {
        // This loop will cycle through the breathing phases indefinitely.
        while (true)
        {
            foreach (var phase in breathingPhases)
            {
                yield return StartCoroutine(DoPhase(phase));
            }
        }
    }

    private IEnumerator DoPhase(BreathingPhase phase)
    {
        // --- Phase Initialization ---
        instructionText.text = phase.instruction;
        keySpriteImage.gameObject.SetActive(true);
        tickDisplayImage.gameObject.SetActive(false);
        _keyAnimationCoroutine = StartCoroutine(AnimateKeyPrompt(phase));

        float timer = phase.duration;
        
        // For HOLD phases
        float stressTickTimer = 0f;

        // For TAP phases: tracks if the user has successfully tapped AT ALL during this phase.
        bool hasTappedSuccessfully = false;

        // --- Main Phase Loop ---
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            if (progressBar != null)
                progressBar.fillAmount = 1f - (timer / phase.duration);

            // --- Handle Input Based on Phase Type ---
            if (phase.phaseType == BreathingPhaseType.Hold)
            {
                stressTickTimer += Time.deltaTime;
                bool isHolding = Input.GetKey(phase.keyToPress);

                // Apply stress changes periodically for HOLDING
                if (stressTickTimer >= stressTickRate)
                {
                    stressTickTimer = 0f;
                    if (isHolding)
                        stressValue.Decrease(stressChangeAmount);
                    else
                        stressValue.Increase(stressChangeAmount);
                }

                // Update UI for HOLDING
                if (isHolding)
                {
                    keySpriteImage.sprite = phase.keyDownSprite;
                    tickDisplayImage.sprite = correctTickSprite;
                    tickDisplayImage.gameObject.SetActive(true);
                }
                else
                {
                    tickDisplayImage.gameObject.SetActive(false);
                }
            }
            else if (phase.phaseType == BreathingPhaseType.Tap)
            {
                // We only care about the FIRST successful tap.
                if (!hasTappedSuccessfully && Input.GetKeyDown(phase.keyToPress))
                {
                    hasTappedSuccessfully = true;

                    // Stop the blinking key animation and show a successful state.
                    if (_keyAnimationCoroutine != null)
                    {
                        StopCoroutine(_keyAnimationCoroutine);
                        _keyAnimationCoroutine = null;
                    }

                    keySpriteImage.sprite = phase.keyDownSprite;
                    tickDisplayImage.sprite = correctTickSprite;
                    tickDisplayImage.gameObject.SetActive(true);
                }
            }

            yield return null;
        }
        
        // --- Phase Cleanup & Final Stress Adjustment for TAP ---
        if (_keyAnimationCoroutine != null)
        {
            StopCoroutine(_keyAnimationCoroutine);
            _keyAnimationCoroutine = null;
        }

        // For TAP phases, apply stress change ONCE at the end.
        if (phase.phaseType == BreathingPhaseType.Tap)
        {
            if (hasTappedSuccessfully)
            {
                Debug.Log("Tap Successful! Stress decreased.");
                stressValue.Decrease(stressChangeAmount);
            }
            else
            {
                Debug.Log("Tap Missed! Stress increased.");
                stressValue.Increase(stressChangeAmount);
            }
        }

        // Reset UI for the next phase
        keySpriteImage.sprite = phase.keyUpSprite;
        tickDisplayImage.gameObject.SetActive(false);
        timerText.text = "0";
        if (progressBar != null) progressBar.fillAmount = 1f;
    }

    private IEnumerator AnimateKeyPrompt(BreathingPhase phase)
    {
        while (true)
        {
            keySpriteImage.sprite = phase.keyUpSprite;
            yield return new WaitForSeconds(keyAnimationSpeed);
            keySpriteImage.sprite = phase.keyDownSprite;
            yield return new WaitForSeconds(keyAnimationSpeed);
        }
    }
}