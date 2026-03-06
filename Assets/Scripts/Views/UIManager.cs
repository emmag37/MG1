/**
 * Insert File Description
 * 
 */

// continue editing:
    // game over does not launch game over canvas

using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // (Editable in Unity Inspector)
    // ==================================================

    // Canvases
    [SerializeField] private GameObject homeCanvas;
    [SerializeField] private GameObject gamePlayCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject settingsCanvas;

    // text
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text gameOverScoreText;


    // ==================================================
    // Public Properties
    // ==================================================


    // ==================================================
    // Events
    // ==================================================
    public event Action StartGame;
    public event Action EndGame;
    public event Action PauseGame;
    public event Action ResumeGame;


    // ==================================================
    // Private Fields
    // ==================================================
    private GameObject currentCanvas;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Start()
    {
        currentCanvas = homeCanvas;
    }

    // ==================================================
    // Public Methods
    // ==================================================
    public void GameOver(int score)
    {
        gameOverScoreText.text = $"{score}";    // update the score text
        ChangeCanvas(gameOverCanvas);
    }

    public void UpdateScoreText(int score, int highScore)
    {
        scoreText.text = $"{score}";
        highScoreText.text = $"{highScore}";
    }


    // ================================
    // Button Methods
    // ================================

    public void OnPlayClicked()
    {
        // assume current canvas is the home screen or game over screen

        ChangeCanvas(gamePlayCanvas);
        StartGame?.Invoke();
    }

    public void OnHomeClicked()
    {
        // assume game over screen

        ChangeCanvas(homeCanvas);
    }

    public void OnReplayClicked()
    {
        // assume the current canvas is the settings screen
            // leave as not pop up for now, add that back in later

        EndGame?.Invoke();
        ChangeCanvas(gamePlayCanvas);
        StartGame?.Invoke();
    }

    public void OnExitGameClicked()
    {
        // assume the current canvas is the settings screen

        EndGame?.Invoke();
        ChangeCanvas(homeCanvas);
    }

    public void OnCloseSettingsClicked()
    {
        // assume the current canvas is the settings screen

        ChangeCanvas(gamePlayCanvas);
        ResumeGame?.Invoke();
    }

    public void OnSettingsClicked()
    {
        // assume the current canvas is the game play screen

        PauseGame?.Invoke();
        ChangeCanvas(settingsCanvas);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void ChangeCanvas(GameObject newCanvas)
    {
        currentCanvas.SetActive(false);
        newCanvas.SetActive(true);

        currentCanvas = newCanvas;
    }

}
