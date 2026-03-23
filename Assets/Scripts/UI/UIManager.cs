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
    [SerializeField] private ViewController viewController;

    // ==================================================
    // Events
    // ==================================================
    public event Action StartGame;
    public event Action EndGame;
    public event Action PauseGame;
    public event Action ResumeGame;

    public event Action<string> UpdateUsername;

    // ==================================================
    // Private Fields
    // ==================================================
    private bool activeGame = false;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(hudController != null, "HUD controller not set");
        Debug.Assert(hudController != null, "HUD controller not set");
    }

    void Awake()    // moved some
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Debug.Assert(Instance == this, "Another UI manager was set as Instance first");

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

    public void InitUsername(string name)
    {
        //ProfileView profile = (ProfileView)GetPopUpView(PopUpViewType.Profile);
        //profile.SetUsername(name);
    }

    public void RecieveUsername(string name)
    {
        UpdateUsername?.Invoke(name);
    }


    // view controller functions
    public void ShowView(BaseViewType type, ViewData data = null)
    {
        if (activeGame) CloseGame();

        viewController.ShowView(type, data);

        if (type == BaseViewType.GamePlay) OpenGame();
    }
    
    public void PushOverlay(PopUpViewType type)
    {
        if (type == PopUpViewType.Pause)
        {
            PauseGame?.Invoke();
        }

        viewController.PushOverlay(type);
    }


    public void PopOverlay()
    {
        PopUpViewType overlay = viewController.PopOverlay();

        if (overlay == PopUpViewType.Pause)
        {
            ResumeGame?.Invoke();
        }
    }

    public void ClearOverlay()
    {
        PopUpViewType finalOverlay = viewController.ClearOverlay();

        if (finalOverlay == PopUpViewType.Pause)
        {
            ResumeGame?.Invoke();
        }
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void CloseGame()
    {
        Debug.Assert(activeGame, $"Attempted exiting gameplay from inactive game state");

        EndGame?.Invoke();
        hudController.Hide();

        activeGame = false;
    }

    private void OpenGame()
    {
        Debug.Assert(!activeGame, $"Attempted starting new gameplay from active game state");

        hudController.Show();
        StartGame?.Invoke();

        activeGame = true;
    }

}
