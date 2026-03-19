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
    // Public Properties
    // ==================================================

    public static UIManager Instance { get; private set; }

    // ==================================================
    // Inspector Fields
    // ==================================================

    // Views
    [SerializeField] private HomeView homeView;
    [SerializeField] private PlayView gamePlayView;
    [SerializeField] private GameOverView gameOverView;
    [SerializeField] private SettingsView settingsView;

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
    private Stack<UIView> viewStack = new Stack<UIView>();


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(homeView != null, "Home view not set");
        Debug.Assert(gamePlayView != null, "Game play view not set");
        Debug.Assert(gameOverView != null, "Game over view not set");
        Debug.Assert(settingsView != null, "Settings view not set");

        Debug.Assert(scoreText != null, "Score text not set");
        Debug.Assert(highScoreText != null, "High score text not set");
        Debug.Assert(gameOverScoreText != null, "Game over score text not set");

        Debug.Assert(playerPreview != null, "Player preview not set");
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Debug.Assert(Instance == this, "Another UI manager was set as Instance first");
    }

    void Start()
    {
        PushView(homeView);
    }


    // ==================================================
    // Public Methods (HUD)
    // ==================================================

    /// <summary>
	/// Updates UI to the Game Over view and score.
	/// </summary>
	/// <param name="score">Score earned during game play.</param>
    public void GameOver(int score)
    {
        gameOverScoreText.text = $"{score}";    // update the score text

        ClearStack();
        PushView(gameOverView);
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


    // ==================================================
    // Public Methods (Views)
    // ==================================================

    /// <summary>
	/// Updates UI and alerts the game manager for a fresh game scene.
	/// </summary>
	/// <param name="activeGame">Describes if there is currently an open game.</param>
    public void LaunchNewGame(bool activeGame)
    {
        if (activeGame)
        {
            EndGame?.Invoke();
        }

        ClearStack();
        PushView(gamePlayView);

        StartGame?.Invoke();
    }

    /// <summary>
	/// Updates UI to return to the home screen.
	/// </summary>
	/// <param name="activeGame">Describes if there is currently an open game.</param>
    public void LaunchHomeScreen(bool activeGame)
    {
        if (activeGame)
        {
            EndGame?.Invoke();
        }

        ClearStack();
        PushView(homeView);
    }

    /// <summary>
	/// Opens the settings pop-up menu.
	/// </summary>
    public void OpenSettings()
    {
        PauseGame?.Invoke();
        PushView(settingsView);
    }

    /// <summary>
	/// Closes the settings pop-up menu.
	/// </summary>
    public void CloseSettings()
    {
        Debug.Log("Close settings");

        PopView();
        ResumeGame?.Invoke();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    // Stack Navigation
    private void PushView(UIView view)
    {
        if (viewStack.Count > 0)
        {
            viewStack.Peek().Show();
        }

        view.Show();
        viewStack.Push(view);
    }
    private void PopView()
    {
        if (viewStack.Count == 0) return;

        UIView top = viewStack.Pop();
        top.Hide();

        if (viewStack.Count > 0)
        {
            viewStack.Peek().Show();
        }
    }
    private void ClearStack()
    {
        while (viewStack.Count > 0)
        {
            UIView view = viewStack.Pop();
            view.Hide();
        }
    }

}
