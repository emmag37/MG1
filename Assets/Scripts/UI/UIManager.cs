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

    [SerializeField] private BaseUIView[] baseViewList;
    [SerializeField] private PopUpUIView[] popUpViewList;

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
    private Dictionary<BaseViewType, BaseUIView> baseViews = new Dictionary<BaseViewType, BaseUIView>();
    private Dictionary<PopUpViewType, PopUpUIView> popUpViews = new Dictionary<PopUpViewType, PopUpUIView>();

    private BaseUIView currentView;
    private PopUpUIView overlayView;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(hudController != null, "HUD controller not set");
        Debug.Assert(baseViewList.Length > 0, "Base view list not initialized");
        Debug.Assert(popUpViewList.Length > 0, "Pop up view list not initialized");

        foreach (BaseUIView view in baseViewList)
        {
            Debug.Assert(view != null, $"View in base view list is not initialized");
        }

        foreach (PopUpUIView view in popUpViewList)
        {
            Debug.Assert(view != null, $"View in pop up view list is not initialized");
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Debug.Assert(Instance == this, "Another UI manager was set as Instance first");

        // initialize view dictionaries
        foreach (BaseUIView view in baseViewList)
        {
            if (baseViews.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate base view type: {view.Type}");
                continue;
            }

            baseViews.Add(view.Type, view);
        }

        foreach (PopUpUIView view in popUpViewList)
        {
            if (popUpViews.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate pop up view type: {view.Type}");
                continue;
            }

            popUpViews.Add(view.Type, view);
        }


    }

    void Start()
    {
        currentView = GetBaseView(BaseViewType.Home);
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
        Debug.Assert(currentView.Type == BaseViewType.GamePlay, $"Game over called from invalid view {currentView.Type}");

        GameOverView view = (GameOverView)GetBaseView(BaseViewType.GameOver);         // see if there is a better way, overload Show()?
        view.UpdateGameOverScoreText(score);

        hudController.Hide();
        ShowView(BaseViewType.GameOver);

        Debug.Assert(currentView != null && currentView.Type == BaseViewType.GameOver, "Game over not set");
    }

    public void ShowView(BaseViewType type)
    {
        Debug.Assert(currentView != null, "Current view not set");

        if (overlayView != null) PopOverlay();
        Debug.Assert(overlayView == null, "Dangling overlay view");

        if (currentView.Type == BaseViewType.GamePlay && type != BaseViewType.GameOver)
        {
            Debug.Assert(currentView.Type == BaseViewType.GamePlay, $"Attempted exiting gameplay from invalid view {currentView.Type}");

            EndGame?.Invoke();
            hudController.Hide();
        }

        currentView.Hide();

        currentView = GetBaseView(type);
        currentView.Show();

        if (type == BaseViewType.GamePlay)
        {
            hudController.Show();
            StartGame?.Invoke();
        }

        Debug.Assert(currentView != null, "Current view not set");
        Debug.Assert(currentView.Type == type, $"Show type mismatch. Expected: {type}, Found: {currentView.Type}");
        
    }

    public void PushOverlay(PopUpViewType type)
    {
        Debug.Assert(overlayView == null, "Cannot push overlay until current one is resolved.");

        if (type == PopUpViewType.Pause)
        {
            PauseGame?.Invoke();
        }

        overlayView = GetPopUpView(type);
        overlayView.Show();

        Debug.Assert(overlayView != null, "Overlay view not set");
        Debug.Assert(overlayView.Type == type, $"Push type mismatch. Expected: {type}, Found: {overlayView.Type}");
    }

    public void PopOverlay()
    {
        Debug.Assert(overlayView != null, "Attempted to pop null overlay");

        if (overlayView.Type == PopUpViewType.Pause)
        {
            ResumeGame?.Invoke();
        }

        overlayView.Hide();
        overlayView = null;

        Debug.Assert(overlayView == null, "Overlay view not popped");
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private BaseUIView GetBaseView(BaseViewType type)
    {
        BaseUIView view;
        if (!baseViews.TryGetValue(type, out view))
        {
            Debug.LogError($"Could not access base view for type {type}");
        }

        return view;
    }

    private PopUpUIView GetPopUpView(PopUpViewType type)
    {
        PopUpUIView view;
        if (!popUpViews.TryGetValue(type, out view))
        {
            Debug.LogError($"Could not access pop up view for type {type}");
        }

        return view;
    }
}
