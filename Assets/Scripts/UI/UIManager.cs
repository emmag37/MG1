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
    [SerializeField] private UIView[] viewList;

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
    private Dictionary<ViewType, UIView> views = new Dictionary<ViewType, UIView>();

    private UIView currentView;
    private UIView overlayView; // can turn this into a stack once you define pop up views


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(hudController != null, "HUD controller not set");
        Debug.Assert(viewList.Length > 0, "View list not initialized");

        foreach (UIView view in viewList)
        {
            Debug.Assert(view != null, $"View in view list is not initialized");
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Debug.Assert(Instance == this, "Another UI manager was set as Instance first");

        foreach (UIView view in viewList)
        {
            if (views.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate view type: {view.Type}");
                continue;
            }

            views.Add(view.Type, view);
        }
    }

    void Start()
    {
        currentView = GetView(ViewType.Home);
        currentView.Show();

        Debug.Assert(currentView != null, "Current view was not initialized properly");
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
        Debug.Assert(currentView.Type == ViewType.GamePlay, $"Game over called from invalid view {currentView.Type}");

        GameOverView view = (GameOverView)GetView(ViewType.GameOver);         // see if there is a better way, overload Show()?
        view.UpdateGameOverScoreText(score);

        hudController.Hide();
        ShowView(ViewType.GameOver);

        Debug.Assert(currentView != null && currentView.Type == ViewType.GameOver, "Game over not set");
    }

    public void ShowView(ViewType type)
    {
        Debug.Assert(currentView != null, "Current view not set");

        if (overlayView != null) PopOverlay();
        Debug.Assert(overlayView == null, "Dangling overlay view");

        if (currentView.Type == ViewType.GamePlay && type != ViewType.GameOver)
        {
            Debug.Assert(currentView.Type == ViewType.GamePlay, $"Attempted exiting gameplay from invalid view {currentView.Type}");

            EndGame?.Invoke();
            hudController.Hide();
        }

        currentView.Hide();
        ViewType oldType = currentView.Type;

        currentView = GetView(type);
        currentView.Show();

        if (type == ViewType.GamePlay)
        {
            Debug.Assert(oldType != ViewType.Settings, $"Attempted showing gameplay from invalid view {oldType}");

            hudController.Show();
            StartGame?.Invoke();
        }

        Debug.Assert(currentView != null, "Current view not set");
        Debug.Assert(currentView.Type == type, $"Show type mismatch. Expected: {type}, Found: {currentView.Type}");
        
    }

    public void PushOverlay(ViewType type)
    {
        Debug.Assert(overlayView == null, "Cannot push overlay until current one is resolved.");

        if (currentView.Type == ViewType.GamePlay)
        {
            PauseGame?.Invoke();
        }

        overlayView = GetView(type);
        overlayView.Show();

        Debug.Assert(overlayView != null, "Current view not set");
        Debug.Assert(overlayView.Type == type, $"Push type mismatch. Expected: {type}, Found: {overlayView.Type}");
    }

    public void PopOverlay()
    {
        Debug.Assert(overlayView != null, "Attempted to pop null overlay");

        overlayView.Hide();
        overlayView = null;

        if (currentView.Type == ViewType.GamePlay)
        {
            ResumeGame?.Invoke();
        }

        Debug.Assert(overlayView == null, "Overlay view not popped");
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private UIView GetView(ViewType type)
    {
        UIView view;
        if (!views.TryGetValue(type, out view))
        {
            Debug.LogError($"Could not access view for type {type}");
        }

        return view;
    }
}
