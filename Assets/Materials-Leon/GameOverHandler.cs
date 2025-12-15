using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    public Coffeemetertimerhealth coffeemetertimerhealth;
    public TextMeshProUGUI finalScoreText;
    public Canvas hudCanvas;
    public Canvas pausedCanvas;
    public Canvas gameOverCanvas;
    public Button nextSceneButton;
    public string nextSceneName = "MainMenu"; // Scene to load when button is clicked

    private bool gameOverTriggered = false;

    void Start()
    {
        // Hook up button listener if assigned
        if (nextSceneButton != null)
            nextSceneButton.onClick.AddListener(LoadNextScene);
    }

    void Update()
    {
        // Check if coffee meter is depleted
        if (coffeemetertimerhealth != null && coffeemetertimerhealth.coffe_meter_health <= 0 && !gameOverTriggered)
        {
            gameOverTriggered = true;
            TriggerGameOver();
        }

       // Display final score from Coffeemetertimerhealth
        if (finalScoreText != null && coffeemetertimerhealth != null)
            finalScoreText.text = "Your Score: " + Mathf.RoundToInt(coffeemetertimerhealth.score).ToString();
        
    }

    void TriggerGameOver()
    {
        // Disable HUD and pause canvases
        if (hudCanvas != null)
            hudCanvas.enabled = false;
        if (pausedCanvas != null)
            pausedCanvas.enabled = false;
        
        // Enable game over canvas
        if (gameOverCanvas != null)
            gameOverCanvas.enabled = true;
        
        // Update final score display
        if (finalScoreText != null && coffeemetertimerhealth != null)
            finalScoreText.text = "Your Score: " + Mathf.RoundToInt(coffeemetertimerhealth.score).ToString();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
