using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// Controls the screen that is displayed.
///
/// Also contains all button functions.
/// </summary>
public class UIManager : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================

    // Canvases
    [SerializeField] private GameObject homeCanvas;
    [SerializeField] private GameObject gamePlayCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject settingsCanvas;

    // Text
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text gameOverScoreText;

    // Images
    [SerializeField] private SpriteView playerPreview;

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
    private Stack<GameObject> canvasStack = new Stack<GameObject>();


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(homeCanvas != null, "Home canvas not set");
        Debug.Assert(gamePlayCanvas != null, "Game play canvas not set");
        Debug.Assert(gameOverCanvas != null, "Game over canvas not set");
        Debug.Assert(settingsCanvas != null, "Settings canvas not set");

        Debug.Assert(scoreText != null, "Score text not set");
        Debug.Assert(highScoreText != null, "High score text not set");
        Debug.Assert(gameOverScoreText != null, "Game over score text not set");

        Debug.Assert(playerPreview != null, "Player preview not set");
    }

    void Start()
    {
        PushCanvas(homeCanvas);
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Updates UI to the Game Over canvas and score.
	/// </summary>
	/// <param name="score">Score earned during game play.</param>
    public void GameOver(int score)
    {
        gameOverScoreText.text = $"{score}";    // update the score text

        PushCanvas(gameOverCanvas);
    }

    /// <summary>
	/// Updates the score and high score UI.
	/// </summary>
	/// <param name="score">New score earned during game.</param>
	/// <param name="highScore">Current high score for this user.</param>
    public void UpdateScoreText(int score, int highScore)
    {
        scoreText.text = $"{score}";
        highScoreText.text = $"{highScore}";
    }

    /// <summary>
	/// Updates the player preview sprite with the given color.
	/// </summary>
	/// <param name="color">New sprite color.</param>
    public void UpdatePlayerPreview(CellColor color)
    {
        playerPreview.SetSprite(SpriteDatabase.Instance.GetSprite(color));
    }


    // ================================
    // Button Methods
    // ================================

    public void OnPlayClicked()
    {
        ClearStack();
        PushCanvas(gamePlayCanvas);

        StartGame?.Invoke();
    }

    public void OnHomeClicked()
    {
        ClearStack();
        PushCanvas(homeCanvas);
    }

    public void OnReplayClicked()
    {
        EndGame?.Invoke();

        PopCanvas();

        StartGame?.Invoke();
    }

    public void OnExitGameClicked()
    {
        EndGame?.Invoke();

        ClearStack();
        PushCanvas(homeCanvas);
    }

    public void OnCloseSettingsClicked()
    {
        PopCanvas();

        ResumeGame?.Invoke();
    }

    public void OnSettingsClicked()
    {
        PauseGame?.Invoke();

        PushCanvas(settingsCanvas);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    // Stack Navigation
    private void PushCanvas(GameObject canvas)
    {
        if (canvasStack.Count > 0)
        {
            canvasStack.Peek().SetActive(false);
        }

        canvas.SetActive(true);
        canvasStack.Push(canvas);
    }
    private void PopCanvas()
    {
        if (canvasStack.Count == 0) return;

        GameObject top = canvasStack.Pop();
        top.SetActive(false);

        if (canvasStack.Count > 0)
        {
            canvasStack.Peek().SetActive(true);
        }
    }
    private void ClearStack()
    {
        while (canvasStack.Count > 0)
        {
            GameObject canvas = canvasStack.Pop();
            canvas.SetActive(false);
        }
    }

}
