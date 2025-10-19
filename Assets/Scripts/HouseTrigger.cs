using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HouseTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            ShowGameOverUI("🏠 You reached the house! Level Complete!");
        }
    }

    private void ShowGameOverUI(string message)
    {
        // Pause the game
        Time.timeScale = 0f;

        // Create Canvas
        GameObject canvasGO = new GameObject("GameOverCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create Panel
        GameObject panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.75f); // semi-transparent black

        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // Create Text
        GameObject textGO = new GameObject("GameOverText");
        textGO.transform.SetParent(panelGO.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 60;
        tmp.color = Color.white;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        // Optional: Restart button
        GameObject buttonGO = new GameObject("RestartButton");
        buttonGO.transform.SetParent(panelGO.transform, false);
        Button button = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = Color.white;

        RectTransform buttonRT = buttonGO.GetComponent<RectTransform>();
        buttonRT.sizeDelta = new Vector2(200, 60);
        buttonRT.anchoredPosition = new Vector2(0, -100);

        // Button text
        GameObject btnTextGO = new GameObject("Text");
        btnTextGO.transform.SetParent(buttonGO.transform, false);
        TextMeshProUGUI btnTMP = btnTextGO.AddComponent<TextMeshProUGUI>();
        btnTMP.text = "Restart";
        btnTMP.alignment = TextAlignmentOptions.Center;
        btnTMP.fontSize = 30;
        btnTMP.color = Color.black;

        RectTransform btnTextRT = btnTextGO.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        // Restart functionality
        button.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        });
    }
}