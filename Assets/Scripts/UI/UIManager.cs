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
        hudController.UpdateScoreText(0, data.Profile.HighScore);

        viewController.ShowView(BaseViewType.Home, data.Profile);

        if (data.Settings.HasLaunched == 0)
        {
            PushOverlay(PopUpViewType.Tutorial1);
            data.SetLaunched();
        }
    }


    // ==================================================
    // Public Methods
    // ==================================================

    // game functions
    public void ShowGameOver()
    {
        Debug.Log("show game over");

        ShowView(BaseViewType.GameOver);
        hudController.Hide();

        activeGame = false;
    }

    public void UpdateScore(int score)
    {
        hudController.UpdateScoreText(score, data.Profile.HighScore);
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

    public void UpdateAvatar(CellColor color)
    {
        data.SetAvatar((int)color);

        viewController.RefreshOverlay(PopUpViewType.Profile, data.Profile);
        viewController.RefreshView(BaseViewType.Home, data.Profile);
    }

    public void UpdateMusicOn(int on)
    {
        data.SetMusicOn(on);
        EventBus.Publish(new UpdateSettingsEvent());
    }

    public void UpdateEffectsOn(int on)
    {
        data.SetEffectsOn(on);
        EventBus.Publish(new UpdateSettingsEvent());
    }

    // ==================================================
    // View Management Methods
    // ==================================================

    public void ShowView(BaseViewType type)
    {
        EventBus.Publish(new TransitionEvent());

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

            case BaseViewType.Home:
                {
                    if (activeGame) CloseGame();
                    viewController.ShowView(type, data.Profile);
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
        EventBus.Publish(new TransitionEvent());

        switch (type)
        {
            case PopUpViewType.Pause:
                {
                    EventBus.Publish(new PauseGameEvent());
                    viewController.PushOverlay(type, data.Settings);
                    break;
                }

            case PopUpViewType.Profile:
                {
                    viewController.PushOverlay(type, data.Profile);
                    break;
                }

            case PopUpViewType.ChooseAvatar:
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
        EventBus.Publish(new TransitionEvent());

        PopUpViewType overlay = viewController.PopOverlay();

        if (overlay == PopUpViewType.Pause)
        {
            EventBus.Publish(new ResumeGameEvent());
        }
    }

    public void ClearOverlay()
    {
        EventBus.Publish(new TransitionEvent());

        PopUpViewType finalOverlay = viewController.ClearOverlay();

        if (finalOverlay == PopUpViewType.Pause)
        {
            EventBus.Publish(new ResumeGameEvent());
        }
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void CloseGame()
    {
        Debug.Assert(activeGame, $"Attempted exiting gameplay from inactive game state");

        EventBus.Publish(new ExitGameEvent());
        hudController.Hide();

        activeGame = false;
    }

    private void OpenGame()
    {
        Debug.Assert(!activeGame, $"Attempted starting new gameplay from active game state");

        hudController.Show();
        EventBus.Publish(new StartGameEvent());

        activeGame = true;
    }

}
