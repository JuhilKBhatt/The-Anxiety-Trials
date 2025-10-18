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
    [Tooltip("How often (in seconds) to increase or decrease stress.")]
    [SerializeField] private float stressTickRate = 0.5f;

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
        instructionText.text = phase.instruction;
        keySpriteImage.gameObject.SetActive(true);
        tickDisplayImage.gameObject.SetActive(false);

        _keyAnimationCoroutine = StartCoroutine(AnimateKeyPrompt(phase));

        float timer = phase.duration;
        float stressTickTimer = 0f;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            stressTickTimer += Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();

            if (progressBar != null)
                progressBar.fillAmount = 1f - (timer / phase.duration);

            bool correctInput = false;

            if (phase.phaseType == BreathingPhaseType.Hold)
            {
                // Player must hold key
                correctInput = Input.GetKey(phase.keyToPress);
            }
            else if (phase.phaseType == BreathingPhaseType.Tap)
            {
                // Player taps key at least once
                correctInput = Input.GetKeyDown(phase.keyToPress);
            }

            // Apply stress changes every stressTickRate seconds
            if (stressTickTimer >= stressTickRate)
            {
                stressTickTimer = 0f;
                if (correctInput)
                    stressValue.Decrease(0.05f);
                else
                    stressValue.Increase(0.05f);
            }

            // Update key sprite and tick
            if (correctInput)
            {
                if (_keyAnimationCoroutine != null)
                {
                    StopCoroutine(_keyAnimationCoroutine);
                    _keyAnimationCoroutine = null;
                }

                keySpriteImage.sprite = phase.keyDownSprite;
                tickDisplayImage.sprite = correctTickSprite;
                tickDisplayImage.gameObject.SetActive(true);
            }
            else
            {
                if (_keyAnimationCoroutine == null)
                    _keyAnimationCoroutine = StartCoroutine(AnimateKeyPrompt(phase));

                tickDisplayImage.gameObject.SetActive(false);
            }

            yield return null;
        }

        if (_keyAnimationCoroutine != null)
        {
            StopCoroutine(_keyAnimationCoroutine);
            _keyAnimationCoroutine = null;
        }

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