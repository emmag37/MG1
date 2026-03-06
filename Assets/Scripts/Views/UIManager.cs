/**
 * Insert File Description
 * 
 */

// pick up where you left off:
    // link everything in the inspector
    // perform the canvas changing functions

using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    // ==================================================
    // Constants
    // ==================================================


    // ==================================================
    // Inspector Fields
    // (Editable in Unity Inspector)
    // ==================================================

    // Canvases
    [SerializeField] private GameObject homeCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gameSettingsCanvas;

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
    public event Action<> StartGame;
    public event Action<> EndGame;
    public event Action<> PauseGame;
    public event Action<> ResumeGame;


    // ==================================================
    // Private Fields
    // ==================================================


    // ==================================================
    // Public Methods
    // ==================================================
    public void GameOver(int score)
    {
        gameOverScoreText.text = $"{score}";    // update the score text

        // update the ui canvases
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = $"{score}";

        // update high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
            PlayerPrefs.Save();
        }

        highScoreText.text = $"{highScore}";
    }


    // ================================
    // Button Methods
    // ================================

    // only called from home screen
    public void OnPlayButtonClicked()
    {
        StartGame?.Invoke();

        // change the canvas

    }

    // called from game settings and game over
    public void OnReplayButtonClicked(string button)
    {
        EndGame?.Invoke();

        StartGame?.Invoke();

        // change the canvas
    }

    // called from game settings and game over
    public void OnHomeButtonClicked(string button)
    {
        EndGame?.Invoke();

        // change the canvas
    }

    // also a pause
    public void OnSettingsClicked()
    {
        PauseGame?.Invoke();

        // change the canvas
    }

    public void OnSettingsExitClicked()
    {
        ResumeGame?.Invoke();

        // change the canvas
    }


    // ==================================================
    // Private Methods
    // ==================================================



}
