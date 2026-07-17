using UnityEngine;
using System;
using System.Collections.Generic;

// todo: replace settings service with a UI data service

public class UIManager : MonoBehaviour, IUIViewHost
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

    private UIDataService uIDataService;

    private Board board;
    private Tutorial tutorial;

    private ViewController<BaseView, BaseViewType, IUIData> baseViewController;
    private ViewController<PopUpView, PopUpViewType, IUIData> popUpViewController;


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

    public void Initialize(Board board, Tutorial tutorial, UIDataService uIDataService)
    {
        Debug.Assert(!instantiated, "Instance of UIManager already exists.");
        instantiated = true;

        this.uIDataService = uIDataService;

        this.board = board;
        this.tutorial = tutorial;

        baseViewController = new ViewController<BaseView, BaseViewType, IUIData>(baseViewList, BaseViewCapacity, this);
        popUpViewController = new ViewController<PopUpView, PopUpViewType, IUIData>(popUpViewList, PopUpViewCapacity, this);
    }

    // ==================================================
    // Interface Method Delegation
    // ==================================================

    public void PushView<TType>(TType type) where TType : struct, Enum
    {
        if (typeof(TType) == typeof(BaseViewType))
            ShowBaseView((BaseViewType)(object)type);
        else if (typeof(TType) == typeof(PopUpViewType))
            PushOverlay((PopUpViewType)(object)type);
        else
            Debug.LogError($"Unsupported view type: {typeof(TType).Name}");
    }

    public void PopView<TType>() where TType : struct, Enum
    {
        if (typeof(TType) == typeof(PopUpViewType))
            PopOverlay();
        else
            Debug.LogError($"Unsupported view type: {typeof(TType).Name}");
    }

    public void UpdateData(IUIData data)
    {
        if (data is SettingsData settings)
            UpdateSettings(settings);
        else if (data is ProfileData profile)
            UpdateProfile(profile);
        else
            Debug.LogError($"Unsupported ui data type: {typeof(IUIData).Name}");
    }


    // ==================================================
    // View Controller Methods
    // ==================================================

    // rename to show base view
    private void ShowBaseView(BaseViewType type, bool playSound = true)
    {
        if (playSound)
        {
            ButtonPressed?.Invoke();
            Transition?.Invoke();
        }

        if (popUpViewController.Count > 0) popUpViewController.ClearViews();

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
        
        baseViewController.PushView(type, uIDataService.Profile);
    }

    // rename to push pop up view
    private void PushOverlay(PopUpViewType type, bool playSound = true)
    {
        if (playSound)
            ButtonPressed?.Invoke();

        IUIData data = uIDataService.Settings;

        if (type == PopUpViewType.Pause)
        {
            board.PauseGame(true);
        }
        else if (type == PopUpViewType.Profile || type == PopUpViewType.ChooseAvatar)
        {
            data = uIDataService.Profile;
        }

        popUpViewController.PushView(type, data);
    }

    // rename to pop pop up view
    private void PopOverlay()
    {
        ButtonPressed?.Invoke();

        if (popUpViewController.PeekViewType() == PopUpViewType.Pause)
        {
            board.PauseGame(false);
        }

        popUpViewController.PopView();
    }

    // ==================================================
    // UI Data Methods
    // ==================================================

    private void UpdateSettings(SettingsData newSettings)
    {
        // update the actual settings

        // update audio/other settings driven systems
    }

    private void UpdateProfile(ProfileData newProfile)
    {
        // update the profile
            // note: score list will never be updated here

        // update the views
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    private void HandleGameOver(int score, int highScore)
    {
        ScoreData scoreData = new ScoreData(score, highScore);
        baseViewController.PushView(BaseViewType.GameOver, scoreData);
    }


}
