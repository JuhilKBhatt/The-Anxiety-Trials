using UnityEngine;

public class HouseTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("🏠 Player reached the house! Game over / level complete.");

            // ✅ New Unity 6+ API
            GameOver gameOver = FindFirstObjectByType<GameOver>();
            if (gameOver != null)
            {
                gameOver.ShowGameOver();
            }
            else
            {
                Debug.LogWarning("⚠️ No GameOver script found in the scene!");
            }
        }
    }
}