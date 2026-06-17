using UnityEngine;
using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // ==================================================
    // Public Fields
    // ==================================================
    public static UIManager Instance { get; private set; }  // change this to an interface? need for my views

    // ==================================================
    // Events
    // ==================================================
    public event Action<BaseViewType, object> ShowBaseView;

    public event Action<PopUpViewType, object> PushOverlayView;
    public event Action PopOverlayView;
    public event Action ClearOverlayView;

    public event Action SkipTutorial;

    public event Action ButtonPressed;  // eventually move to event bus?
    public event Action Transition;

    // ==================================================
    // Private Fields
    // ==================================================
    private SettingsService settingsService;
    private GameDataService gameDataService;

    private GameManager gameManager;

    private BaseViewType baseState;
    private Stack<PopUpViewType> popUpStack = new Stack<PopUpViewType>();


    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }

    // ==================================================
    // Initialize
    // ==================================================

    public void Initialize(SettingsService settingsService, GameDataService gameDataService, GameManager gameManager)
    {
        this.settingsService = settingsService;
        this.gameDataService = gameDataService;
        this.gameManager = gameManager;
    }


    // ==================================================
    // Update Settings Methods
    // ==================================================

    public void UpdateMusicOn(int on)
    {
        settingsService.SetMusicOn(on == 1? true : false);
    }

    public void UpdateSFXOn(int on)
    {
        settingsService.SetSFXOn(on == 1 ? true : false);
    }

    public bool TryUpdateUsername(string name, out InvalidInputType error)
    {
        return settingsService.TrySetUsername(name, out error);
    }

    public void UpdateAvatar(CellColor color)
    {
        settingsService.SetAvatar(color);
    }

    // ==================================================
    // View Controller Methods
    // ==================================================

    // base views
    public void ShowView(BaseViewType type, bool playSound = true)
    {
        if (playSound)
        {
            ButtonPressed?.Invoke();
            Transition?.Invoke();
        }

        if (popUpStack.Count > 0)
        {
            ClearOverlayView?.Invoke();
            popUpStack.Clear();
        }

        IUserSettings userSettings = settingsService.GetSettings();
        if (type == BaseViewType.GamePlay && !userSettings.HasLaunched)
        {
            settingsService.SetLaunched();

            type = BaseViewType.Tutorial;   // switch to the tutorial sequence
            gameManager.RunTutorial();
        }
        else if (type == BaseViewType.Tutorial) // shortcut for skip - only called when tut is active
        {
            SkipTutorial?.Invoke();
            gameManager.SkipTutorial();
            return;
        }
        else if (type == BaseViewType.GamePlay) {
            if (baseState == BaseViewType.Home && gameManager.ActiveGame)
                gameManager.ResumeGame();   // this is getting run when you want to restart
            else 
                gameManager.StartGame();    // should reset the game state automatically
        }
        
        ShowBaseView?.Invoke(type, userSettings);
        baseState = type;
    }

    // pop up views
    public void PushOverlay(PopUpViewType type, bool playSound = true)
    {
        if (playSound)
            ButtonPressed?.Invoke();

        IRuntimeData data = settingsService.GetSettings();

        if (type == PopUpViewType.Pause)
        {
            gameManager.PauseGame();
        }
        else if (type == PopUpViewType.Profile)
        {
            IUserSettings settingsData = (IUserSettings)data;
            IGameData gameData = gameDataService.GetGameData();

            data = new AllData(gameData, settingsData);
        }

        PushOverlayView?.Invoke(type, data);
        popUpStack.Push(type);
    }

    public void PopOverlay()
    {
        ButtonPressed?.Invoke();

        Debug.Log("pop overlay");
        if (popUpStack.Peek() == PopUpViewType.Pause)
        {
            Debug.Log("pop pause");
            gameManager.ResumeGame();
        }

        PopOverlayView?.Invoke();
        popUpStack.Pop();
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    private void OnGameOver(GameOverEvent e)
    {
        baseState = BaseViewType.GameOver;
        ShowBaseView?.Invoke(BaseViewType.GameOver, gameDataService.GetGameData());
    }
}
