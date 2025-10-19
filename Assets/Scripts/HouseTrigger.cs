using UnityEngine;

public class HouseTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverObject; // drag Canvas or panel

    private GameOver gameOverScript;
    private bool triggered = false;

    private void Awake()
    {
        if (gameOverScript == null)
            gameOverScript = FindObjectOfType<GameOver>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (gameOverScript != null)
            {
                gameOverScript.ShowGameOver("🏠 You reached the house! Level Complete!");
            }
            else
            {
                Debug.LogWarning("⚠ GameOver script not found on assigned object!");
            }
        }
    }
}