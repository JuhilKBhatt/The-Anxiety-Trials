using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HouseTrigger : MonoBehaviour
{
    private bool triggered = false;

    // This function is called when another object enters a trigger collider attached to this object.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // We only want the trigger to fire once.
        if (triggered) return;

        // Check if the object that entered the trigger is the Player.
        if (other.CompareTag("Player"))
        {
            triggered = true;
            ShowFaintUI(); // Call the function to show the UI.
        }
    }

    private void ShowFaintUI()
    {
        // Pause the game by setting the time scale to 0.
        Time.timeScale = 0f;

        // --- Canvas Setup ---
        // This is the root object for the UI.
        GameObject canvasGO = new GameObject("FaintCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- Panel Setup ---
        // This dark, semi-transparent panel will be the background.
        GameObject panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.85f); // Dark semi-transparent background

        // Make the panel fill the entire screen.
        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // --- "You Fainted!" Text ---
        GameObject textGO = new GameObject("FaintedText");
        textGO.transform.SetParent(panelGO.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "You Win!";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 100;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        // Position the text at the top of the screen.
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.sizeDelta = new Vector2(800, 120);
        textRT.anchoredPosition = new Vector2(0, 180);

        // --- Restart Button ---
        // Create the button's GameObject.
        GameObject restartButtonGO = new GameObject("RestartButton");
        restartButtonGO.transform.SetParent(panelGO.transform, false);
        Image restartButtonImage = restartButtonGO.AddComponent<Image>();
        restartButtonImage.color = new Color(1f, 0.76f, 0.22f); // Yellow color: #FFC338
        Button restartButton = restartButtonGO.AddComponent<Button>();

        // Set button size and position.
        RectTransform restartButtonRT = restartButtonGO.GetComponent<RectTransform>();
        restartButtonRT.sizeDelta = new Vector2(320, 80);
        restartButtonRT.anchoredPosition = new Vector2(0, 15);

        // Add text to the button.
        GameObject restartBtnTextGO = new GameObject("Text");
        restartBtnTextGO.transform.SetParent(restartButtonGO.transform, false);
        TextMeshProUGUI restartBtnTMP = restartBtnTextGO.AddComponent<TextMeshProUGUI>();
        restartBtnTMP.text = "Restart";
        restartBtnTMP.alignment = TextAlignmentOptions.Center;
        restartBtnTMP.fontSize = 40;
        restartBtnTMP.color = Color.black;

        // Add the restart functionality.
        restartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f; // Resume game time before loading scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });


        // --- Quit Button ---
        // Create the button's GameObject.
        GameObject quitButtonGO = new GameObject("QuitButton");
        quitButtonGO.transform.SetParent(panelGO.transform, false);
        Image quitButtonImage = quitButtonGO.AddComponent<Image>();
        quitButtonImage.color = new Color(0.9f, 0.35f, 0.32f); // Red color: #E65952
        Button quitButton = quitButtonGO.AddComponent<Button>();

        // Set button size and position.
        RectTransform quitButtonRT = quitButtonGO.GetComponent<RectTransform>();
        quitButtonRT.sizeDelta = new Vector2(320, 80);
        quitButtonRT.anchoredPosition = new Vector2(0, -85);

        // Add text to the button.
        GameObject quitBtnTextGO = new GameObject("Text");
        quitBtnTextGO.transform.SetParent(quitButtonGO.transform, false);
        TextMeshProUGUI quitBtnTMP = quitBtnTextGO.AddComponent<TextMeshProUGUI>();
        quitBtnTMP.text = "Quit";
        quitBtnTMP.alignment = TextAlignmentOptions.Center;
        quitBtnTMP.fontSize = 40;
        quitBtnTMP.color = Color.black;

        // Add the quit functionality.
        quitButton.onClick.AddListener(QuitGame);
    }

    // A separate function to handle quitting the game.
    private void QuitGame()
    {
        // This line quits the application.
        // Note: It only works in a built game, not in the Unity Editor.
        Application.Quit();

        // The following line stops play mode in the Unity Editor.
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}