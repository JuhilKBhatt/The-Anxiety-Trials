using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class BreathingPhase
{
    public string instruction;      
    public float duration = 4f;     
    public KeyCode keyToPress;      
    public Sprite keyUpSprite;      
    public Sprite keyDownSprite;    
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
    [Tooltip("How often (in seconds) to increase or decrease stress.")]
    [SerializeField] private float stressTickRate = 0.5f;

    private Coroutine _keyAnimationCoroutine;

    private void Start()
    {
        if (breathingPhases == null || breathingPhases.Length == 0)
        {
            Debug.LogError("BreathingUI: No breathing phases have been set up in the Inspector!");
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

            if (stressTickTimer >= stressTickRate)
            {
                stressTickTimer = 0f; // Reset the tick timer

                // ✨ Check if StressBar exists before trying to use it
                if (StressBar.Instance != null)
                {
                    if (Input.GetKey(phase.keyToPress))
                    {
                        StressBar.Instance.DecreaseStress(); // User is doing it right
                    }
                    else
                    {
                        StressBar.Instance.IncreaseStress(); // User is missing the key
                    }
                }
            }

            if (Input.GetKey(phase.keyToPress))
            {
                if (_keyAnimationCoroutine != null)
                {
                    StopCoroutine(_keyAnimationCoroutine);
                    _keyAnimationCoroutine = null;
                }

                keySpriteImage.sprite = phase.keyDownSprite;

                // ✅ Show tick sprite
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