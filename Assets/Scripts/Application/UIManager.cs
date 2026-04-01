using UnityEngine;
using System;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
    // ==================================================
    // Public Fields
    // ==================================================
    public static UIManager Instance { get; private set; }

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

    // ==================================================
    // Private Fields
    // ==================================================
    private BaseViewType baseState;
    private Stack<PopUpViewType> popUpStack = new Stack<PopUpViewType>();


    // ================================
    // Unity Lifecycle Methods - move this to game initializer
    // ================================

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowBaseView?.Invoke(BaseViewType.Home, new NoData());
        baseState = BaseViewType.Home;

        /* replace with new system
        if (data.Settings.HasLaunched == 0)
        {
            PushOverlayView(PopUpViewType.Tutorial, new NoData());
            data.SetLaunched();
        }*/
    }


    // ==================================================
    // Public Methods
    // ==================================================

    public void UpdateUsername(string name)
    {
        // set username in data
    }

    public void UpdateAvatar(CellColor color)
    {
        // set avatar in data
        // refresh views
    }

    public void UpdateMusicOn(int on)
    {
        // set music in data
        EventBus.Publish(new UpdateSettingsEvent());    // keep/delete?
    }

    public void UpdateEffectsOn(int on)
    {
        // set effects in data
        EventBus.Publish(new UpdateSettingsEvent());    // keep/delete?
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

        // prepare data for the view - always user settings

        baseState = type;
        ShowBaseView?.Invoke(type, new UserSettings());
    }

    // pop up views
    public void PushOverlay(PopUpViewType type)
    {
        if (type == PopUpViewType.Pause)
        {
            gameManager.PauseGame();
        }
        
        // prepare data

        popUpStack.Push(type);
        PushOverlayView?.Invoke(type, new UserSettings());
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
