using UnityEngine;
using System;
using System.Collections.Generic;

// to-do:
    // need to reconfigure passing data to the views
    // add back data entry for profile view, choose avatar view, pause view

// point of connection with the game manager and data manager
public class UIManager : MonoBehaviour
{
    // ==================================================
    // Public Fields
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
    private GameManager gameManager; 

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
            viewController.PushOverlay(PopUpViewType.Tutorial1, new NoData());
            data.SetLaunched();
        }
    }

    void OnEnable()
    {
        EventBus.Subscribe<GameOverEvent>(OnGameOver);

        gameManager.UpdateScore += OnUpdateScore;
        gameManager.UpdatePlayerPreview += OnUpdatePlayerPreview;

        // subscribe to view controller events
        viewController.StartPressed += HandleStartPressed;
        viewController.ExitGamePressed += HandleExitGamePressed;
        viewController.PausePressed += HandlePausePressed;
        viewController.ResumePressed += HandleResumePressed;
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

        gameManager.UpdateScore -= OnUpdateScore;
        gameManager.UpdatePlayerPreview -= OnUpdatePlayerPreview;

        // subscribe to view controller events
        viewController.StartPressed -= HandleStartPressed;
        viewController.ExitGamePressed -= HandleExitGamePressed;
        viewController.PausePressed -= HandlePausePressed;
        viewController.ResumePressed -= HandleResumePressed;
    }


    // ==================================================
    // Public Methods
    // ==================================================

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
    // View Controller Handlers
    // ==================================================

    private void HandleStartPressed()
    {
        hudController.UpdateScoreText(0, data.Profile.HighScore);
        hudController.Show();

        gameManager.StartGame();
    }

    private void HandleExitGamePressed()
    {
        hudController.Hide();

        gameManager.ExitGame();
    }

    private void HandlePausePressed()
    {
        gameManager.PauseGame();
    }

    private void HandleResumePressed()
    {
        gameManager.ResumeGame();
    }

    // ==================================================
    // Game State Event Handlers
    // ==================================================

    private void OnGameOver(GameOverEvent e)
    {
        hudController.Hide();
        viewController.ShowView(BaseViewType.GameOver, data.Profile);
    }


    // ==================================================
    // Game Updates Event Handlers
    // ==================================================

    private void OnUpdateScore(int score, int highScore)
    {
        hudController.UpdateScoreText(score, highScore);
    }

    private void OnUpdatePlayerPreview(CellColor color)
    {
        hudController.UpdatePlayerPreviewSprite(color);
    }
}
