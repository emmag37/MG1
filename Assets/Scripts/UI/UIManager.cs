using UnityEngine;
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
    [SerializeField] private HUDController hudController;

    [SerializeField] private HomeView homeView;
    [SerializeField] private PlayView gamePlayView;
    [SerializeField] private GameOverView gameOverView;
    [SerializeField] private SettingsView settingsView;

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
        Debug.Assert(hudController != null, "HUD controller not set");

        Debug.Assert(homeView != null, "Home view not set");
        Debug.Assert(gamePlayView != null, "Game play view not set");
        Debug.Assert(gameOverView != null, "Game over view not set");
        Debug.Assert(settingsView != null, "Settings view not set");
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
    // Public Methods
    // ==================================================

    public void UpdateScore(int score, int highScore)
    {
        hudController.UpdateScoreText(score, highScore);
    }

    public void UpdatePlayerPreview(CellColor color)
    {
        hudController.UpdatePlayerPreviewSprite(color);
    }

    /// <summary>
	/// Updates UI to the Game Over view and score.
	/// </summary>
	/// <param name="score">Score earned during game play.</param>
    public void GameOver(int score)
    {
        gameOverView.UpdateGameOverScoreText(score);

        ClearStack();
        hudController.Hide();

        PushView(gameOverView);
    }

    /// <summary>
    /// Updates UI and alerts the game manager for a fresh game scene.
    /// </summary>
    /// <param name="activeGame">Describes if there is currently an open game.</param>
    public void LaunchNewGame(bool activeGame)
    {
        if (activeGame)
        {
            EndGame?.Invoke();
            hudController.Hide();
        }

        ClearStack();
        PushView(gamePlayView);
        hudController.Show();

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
            hudController.Hide();
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
