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

    public event Action<TutorialViewType> SwitchTutorialView;
    public event Action CloseTutorialView;

    public event Action ButtonPressed;  // eventually move to event bus?

    // ==================================================
    // Private Fields
    // ==================================================
    private SettingsService settingsService;
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


    // ==================================================
    // Initialize
    // ==================================================

    public void Initialize(SettingsService settingsService, GameManager gameManager, bool hasLaunched)
    {
        this.settingsService = settingsService;
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
    public void ShowView(BaseViewType type, bool playSound = true)
    {
        if (playSound)
            ButtonPressed?.Invoke();

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
    public void PushOverlay(PopUpViewType type, bool playSound = true)
    {
        if (playSound)
            ButtonPressed?.Invoke();

        if (type == PopUpViewType.Pause)
        {
            gameManager.PauseGame();
        }
        else if (type == PopUpViewType.Tutorial)
        {
            SwitchTutorialView?.Invoke(TutorialViewType.Tutorial1);
        }

        IUserSettings userSettings = settingsService.GetSettings();
        PushOverlayView?.Invoke(type, userSettings);

        popUpStack.Push(type);
    }

    public void PopOverlay()
    {
        ButtonPressed?.Invoke();

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
        ButtonPressed?.Invoke();
        SwitchTutorialView?.Invoke(type);
    }
}
