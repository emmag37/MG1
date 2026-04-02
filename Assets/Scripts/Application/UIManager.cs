using UnityEngine;
using System;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
    // ==================================================
    // Public Fields
    // ==================================================
    public static UIManager Instance { get; private set; }  // change this to an interface?

    // ==================================================
    // Events
    // ==================================================
    public event Action<BaseViewType, object> ShowBaseView;

    public event Action<PopUpViewType, object> PushOverlayView;
    public event Action PopOverlayView;

    public event Action<TutorialViewType> SwitchTutorialView;
    public event Action CloseTutorialView;

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SettingsService settingsService;

    // ==================================================
    // Private Fields
    // ==================================================
    private BaseViewType baseState;
    private Stack<PopUpViewType> popUpStack = new Stack<PopUpViewType>();


    // ==================================================
    // Unity Lifecycle Methods - move this to game initializer
    // ==================================================

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowBaseView?.Invoke(BaseViewType.Home, new NoData());
        baseState = BaseViewType.Home;

        IUserSettings userSettings = settingsService.GetSettings();
        if (!userSettings.HasLaunched)
        {
            PushOverlayView?.Invoke(PopUpViewType.Tutorial, new NoData());
            settingsService.SetLaunched(true);
        }
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

    public void UpdateUsername(string name)
    {
        settingsService.SetUsername(name);
    }

    public void UpdateAvatar(CellColor color)
    {
        settingsService.SetAvatar(color);
    }

    // ==================================================
    // View Controller Methods
    // ==================================================

    // base views
    public void ShowView(BaseViewType type)
    {
        if (type == BaseViewType.GamePlay)
        {
            gameManager.StartGame();
        }
        else if (type == BaseViewType.Home && baseState == BaseViewType.GamePlay)
        {
            gameManager.ExitGame();
        }

        if (popUpStack.Count > 0)
        {
            PopOverlayView?.Invoke();
            popUpStack.Pop();
        }

        IUserSettings userSettings = settingsService.GetSettings();
        ShowBaseView?.Invoke(type, userSettings);

        baseState = type;
    }

    // pop up views
    public void PushOverlay(PopUpViewType type)
    {
        if (type == PopUpViewType.Pause)
        {
            gameManager.PauseGame();
        }

        IUserSettings userSettings = settingsService.GetSettings();
        PushOverlayView?.Invoke(type, userSettings);

        popUpStack.Push(type);
    }

    public void PopOverlay()
    {
        if (popUpStack.Peek() == PopUpViewType.Pause)
        {
            gameManager.ResumeGame();
        }
        if (popUpStack.Peek() == PopUpViewType.Tutorial)
        {
            CloseTutorialView?.Invoke();
        }

        PopOverlayView?.Invoke();
        popUpStack.Pop();
    }

    // tutorial views
    public void SwitchTutorial(TutorialViewType type)
    {
        SwitchTutorialView?.Invoke(type);
    }
}
