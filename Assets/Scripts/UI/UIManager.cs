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

    [SerializeField] private BaseView[] baseViewList;
    [SerializeField] private PopUpView[] popUpViewList;

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
    private Dictionary<BaseViewType, BaseView> baseViews = new Dictionary<BaseViewType, BaseView>();
    private Dictionary<PopUpViewType, PopUpView> popUpViews = new Dictionary<PopUpViewType, PopUpView>();

    private BaseView currentView;
    private PopUpView overlayView;  // it's time to implement the stack


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(hudController != null, "HUD controller not set");
        Debug.Assert(baseViewList.Length > 0, "Base view list not initialized");
        Debug.Assert(popUpViewList.Length > 0, "Pop up view list not initialized");

        foreach (BaseView view in baseViewList)
        {
            Debug.Assert(view != null, $"View in base view list is not initialized");
        }

        foreach (PopUpView view in popUpViewList)
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
        foreach (BaseView view in baseViewList)
        {
            if (baseViews.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate base view type: {view.Type}");
                continue;
            }

            baseViews.Add(view.Type, view);
        }

        foreach (PopUpView view in popUpViewList)
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

    public void ShowView(BaseViewType type, ViewData data = null)
    {
        HideCurrentView(type);

        currentView = GetBaseView(type);
        currentView.Show(data);

        ShowGameIfGamePlay(type);

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

    private BaseView GetBaseView(BaseViewType type)
    {
        BaseView view;
        if (!baseViews.TryGetValue(type, out view))
        {
            Debug.LogError($"Could not access base view for type {type}");
        }

        return view;
    }

    private PopUpView GetPopUpView(PopUpViewType type)
    {
        PopUpView view;
        if (!popUpViews.TryGetValue(type, out view))
        {
            Debug.LogError($"Could not access pop up view for type {type}");
        }

        return view;
    }

    private void HideCurrentView(BaseViewType type)
    {
        Debug.Assert(currentView != null, "Current view not set");

        if (overlayView != null) PopOverlay();
        Debug.Assert(overlayView == null, "Dangling overlay view");

        if (currentView.Type == BaseViewType.GamePlay)
        {
            Debug.Assert(currentView.Type == BaseViewType.GamePlay, $"Attempted exiting gameplay from invalid view {currentView.Type}");

            EndGame?.Invoke();
            hudController.Hide();
        }

        currentView.Hide();
    }

    private void ShowGameIfGamePlay(BaseViewType type)
    {
        if (type != BaseViewType.GamePlay) return;

        hudController.Show();
        StartGame?.Invoke();
    }
}
