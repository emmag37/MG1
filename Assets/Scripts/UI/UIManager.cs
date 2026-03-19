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
        Debug.Assert(viewList.Length > 0, "View list not set");
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
        GameOverView view = (GameOverView)GetView(ViewType.GameOver);         // see if there is a better way, overload Show()?
        view.UpdateGameOverScoreText(score);

        hudController.Hide();
        ShowView(ViewType.GameOver);
    }

    public void ShowView(ViewType type)
    {
        if (overlayView != null)
            PopOverlay();

        if (currentView.Type == ViewType.GamePlay)
        {
            EndGame?.Invoke();
            hudController.Hide();
        }

        currentView.Hide();

        currentView = GetView(type);
        currentView.Show();

        if (type == ViewType.GamePlay)
        {
            hudController.Show();
            StartGame?.Invoke();
        }
    }

    public void PushOverlay(ViewType type)
    {
        if (currentView.Type == ViewType.GamePlay)
        {
            PauseGame?.Invoke();
        }

        overlayView = GetView(type);
        overlayView.Show();

    }

    public void PopOverlay()
    {
        overlayView.Hide();
        overlayView = null;

        if (currentView.Type == ViewType.GamePlay)
        {
            ResumeGame?.Invoke();
        }
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
