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
    // Events
    // ==================================================
    public event Action StartGame;
    public event Action EndGame;
    public event Action PauseGame;
    public event Action ResumeGame;

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private HUDController hudController;
    [SerializeField] private ViewController viewController;

    // ==================================================
    // Private Fields
    // ==================================================
    private DataManager data => DataManager.Instance;

    private bool activeGame = false;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(hudController != null, "HUD controller not set");
        Debug.Assert(viewController != null, "View controller not set");
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateScore();

        ShowView(BaseViewType.Home);

        if (data.Settings.HasLaunched == 0)
        {
            PushOverlay(PopUpViewType.Tutorial1);
            data.SetLaunched();
        }
    }


    // ==================================================
    // Public Methods
    // ==================================================

    // hud controller functions
    public void UpdateScore()
    {
        if (data == null) Debug.Log("Data manager not initialized");
        hudController.UpdateScoreText(data.Profile.RecentScore, data.Profile.HighScore);
    }

    public void UpdatePlayerPreview(CellColor color)
    {
        hudController.UpdatePlayerPreviewSprite(color);
    }

    // view functions
    public void UpdateUsername(string name)
    {
        data.SetUsername(name);
    }

    public void UpdateMusicOn(int on)
    {
        data.SetMusicOn(on);
    }

    public void UpdateEffectsOn(int on)
    {
        data.SetEffectsOn(on);
    }

    // view controller functions
    public void ShowView(BaseViewType type)
    {
        if (activeGame) CloseGame();

        switch (type)
        {
            case BaseViewType.GameOver:
                {
                    viewController.ShowView(type, data.Profile);
                    break;
                }

            case BaseViewType.GamePlay:
                {
                    viewController.ShowView(type, new NoData());
                    OpenGame();
                    break;
                }

            default:
                {
                    viewController.ShowView(type, new NoData());
                    break;
                }
        }
    }
    
    public void PushOverlay(PopUpViewType type)
    {
        switch (type)
        {
            case PopUpViewType.Pause:
                {
                    PauseGame?.Invoke();
                    viewController.PushOverlay(type, data.Settings);
                    break;
                }

            case PopUpViewType.Profile:
                {
                    viewController.PushOverlay(type, data.Profile);
                    break;
                }

            default:
                {
                    viewController.PushOverlay(type, new NoData());
                    break;
                }
        }
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
