using UnityEngine;
using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // ==================================================
    // Constants
    // ==================================================
    private const int BaseViewCapacity = 1;
    private const int PopUpViewCapacity = 3;    // think it might be two, but just to be safe

    // ==================================================
    // Events
    // ==================================================
	public event Action ButtonPressed;  // eventually move to event bus?
    public event Action Transition;

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private BaseView[] baseViewList;
    [SerializeField] private PopUpView[] popUpViewList;


    // ==================================================
    // Private Fields
    // ==================================================
    private bool instantiated = false;

    private SettingsService settingsService;
    private GameDataService gameDataService;

    private Board board;
    private Tutorial tutorial;

    private ViewController<BaseView, BaseViewType, IRuntimeData> baseViewController;
    private ViewController<PopUpView, PopUpViewType, IRuntimeData> popUpViewController;


    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    void OnEnable()
    {
        board.FullBoard += HandleGameOver;
    }

    void OnDisable()
    {
        board.FullBoard -= HandleGameOver;
    }

    // ==================================================
    // Initialize
    // ==================================================

    public void Initialize(InitFlag initInfo, Board board, Tutorial tutorial, SettingsService settingsService, GameDataService gameDataService)
    {
        Debug.Assert(!instantiated, "Instance of UIManager already exists.");
        instantiated = true;

        this.settingsService = settingsService;
        this.gameDataService = gameDataService;
        this.board = board;
        this.tutorial = tutorial;

        baseViewController = new ViewController<BaseView, BaseViewType, IRuntimeData>(baseViewList, BaseViewCapacity, this);
        popUpViewController = new ViewController<PopUpView, PopUpViewType, IRuntimeData>(popUpViewList, PopUpViewCapacity, this);
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

    // rename to show base view
    public void ShowView(BaseViewType type, bool playSound = true)
    {
        if (playSound)
        {
            ButtonPressed?.Invoke();
            Transition?.Invoke();
        }

        if (popUpViewController.Count > 0) popUpViewController.ClearViews();

        IUserSettings userSettings = settingsService.GetSettings();
        IGameData gameData = gameDataService.GetGameData();

        if (type == BaseViewType.GamePlay && baseViewController.PeekViewType() == BaseViewType.GamePlay)
        {
            board.PlayGame(restart: true);
        }
        else if (type == BaseViewType.GamePlay)
        {
            board.PlayGame();
        }
        else if (type == BaseViewType.Tutorial && baseViewController.PeekViewType() == BaseViewType.Tutorial)
        {
            tutorial.SkipTutorial();
            return;
        }
        else if (type == BaseViewType.Tutorial)
        {
            tutorial.StartTutorial();
        }

        baseViewController.PushView(type, userSettings);
    }

    // rename to push pop up view
    public void PushOverlay(PopUpViewType type, bool playSound = true)
    {
        if (playSound)
            ButtonPressed?.Invoke();

        IRuntimeData data = settingsService.GetSettings();

        if (type == PopUpViewType.Pause)
        {
            board.PauseGame(true);
        }
        else if (type == PopUpViewType.Profile)
        {
            IUserSettings settingsData = (IUserSettings)data;
            IGameData gameData = gameDataService.GetGameData();

            data = new AllData(gameData, settingsData);
        }

        popUpViewController.PushView(type, data);
    }

    // rename to pop pop up view
    public void PopOverlay()
    {
        ButtonPressed?.Invoke();

        if (popUpViewController.PeekViewType() == PopUpViewType.Pause)
        {
            board.PauseGame(false);
        }

        popUpViewController.PopView();
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    private void HandleGameOver()
    {
        baseViewController.PushView(BaseViewType.GameOver, gameDataService.GetGameData());
    }

    public void HandleProfileUpdate(IUserSettings userSettings)
    {
        baseViewController.UpdateView(BaseViewType.Home, userSettings);
        popUpViewController.UpdateView(PopUpViewType.Profile, userSettings);
    }
}
