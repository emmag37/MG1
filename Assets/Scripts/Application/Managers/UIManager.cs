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

    private Board board;

    private BaseViewType baseState = BaseViewType.None;
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

    public void Initialize(SettingsService settingsService, GameDataService gameDataService, Board board)
    {
        this.settingsService = settingsService;
        this.gameDataService = gameDataService;
        this.board = board;
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

        // why is pause not coming off of the stack????
        if (popUpStack.Count > 0)
        {
            ClearOverlay();
        }

        IUserSettings userSettings = settingsService.GetSettings();
        IGameData gameData = gameDataService.GetGameData();

        if (type == BaseViewType.Tutorial)
        {
            // run tutorial
            Debug.Log("run tutorial");
            board.StartTutorial();
        }
        else if (type == BaseViewType.GamePlay && baseState == BaseViewType.GamePlay) // signal for restart
        {
            board.PlayGame(restart: true);
        }
        else if (type == BaseViewType.GamePlay)
        {
            Debug.Log("attempt game play");
            board.PlayGame();
        }
        else if (baseState == BaseViewType.Tutorial && type == BaseViewType.Tutorial)         // signal for skip - only called when tutorial is active
        {
            SkipTutorial?.Invoke();
            board.SkipTutorial();
            return;
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
            board.PauseGame(true);
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

        if (popUpStack.Peek() == PopUpViewType.Pause)
        {
            board.PauseGame(false);   // unpause
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



    // private functions
    private void ClearOverlay()
    {
        while (popUpStack.Count > 0)
        {
            PopOverlay();
        }
    }
}
