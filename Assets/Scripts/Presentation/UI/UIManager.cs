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
    // Public Fields
    // ==================================================
    public static UIManager Instance { get; private set; }
    public GameManager GameManager; // views need to access

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private HUDController hudController;
    [SerializeField] private ViewController viewController;

    // ==================================================
    // Private Fields
    // ==================================================
    private DataManager data => DataManager.Instance;

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
        viewController.ShowView(BaseViewType.Home, data.Profile);

        if (data.Settings.HasLaunched == 0)
        {
            PushOverlay(PopUpViewType.Tutorial1);
            data.SetLaunched();
        }
    }

    void OnEnable()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<PauseGameEvent>(OnPauseGame);
        EventBus.Subscribe<ResumeGameEvent>(OnResumeGame);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus.Unsubscribe<PauseGameEvent>(OnPauseGame);
        EventBus.Unsubscribe<ResumeGameEvent>(OnResumeGame);
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

    public void ShowView(BaseViewType type) // want to get rid of this wrapper
    {
        EventBus.Publish(new TransitionEvent()); // move this to the view controller

        switch (type)
        {
            case BaseViewType.Home:
                {
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
                    GameManager.PauseGame();
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
            GameManager.ResumeGame();
        }
    }

    public void ClearOverlay()
    {
        EventBus.Publish(new TransitionEvent());

        PopUpViewType finalOverlay = viewController.ClearOverlay();

        if (finalOverlay == PopUpViewType.Pause)
        {
            GameManager.ResumeGame();
        }
    }


    // ==================================================
    // Event Handlers
    // ==================================================

    private void OnStartGame(StartGameEvent e)
    {
        viewController.ShowView(BaseViewType.GamePlay, new NoData());

        hudController.UpdateScoreText(0, data.Profile.HighScore);
        hudController.Show();
    }

    private void OnExitGame(ExitGameEvent e)
    {
        hudController.Hide();
        viewController.ShowView(BaseViewType.Home, data.Profile);
    }

    private void OnGameOver(GameOverEvent e)
    {
        hudController.Hide();
        viewController.ShowView(BaseViewType.GameOver, data.Profile);
    }

    private void OnPauseGame(PauseGameEvent e)
    {
        viewController.PushOverlay(PopUpViewType.Pause, data.Settings);
    }

    private void OnResumeGame(ResumeGameEvent e)
    {
        viewController.PopOverlay();
    }
}
