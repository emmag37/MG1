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
	//public event Action ButtonPressed;  // to remove
    //public event Action Transition;     // to remove

    //public event Action<bool, bool> SettingsUpdate; // to remove

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private BaseView[] baseViewList;
    [SerializeField] private PopUpView[] popUpViewList;


    // ==================================================
    // Private Fields
    // ==================================================
    private bool instantiated = false;

    private UIData data;

    private Board board;
    private Tutorial tutorial;

    private ViewController<BaseView, BaseViewType, IUIData> baseViewController;
    private ViewController<PopUpView, PopUpViewType, IUIData> popUpViewController;


    // ================================
    // Unity Lifecycle
    // ================================

    public void Awake()
    {
        board.FullBoard += HandleGameOver;
    }

    public void OnDestroy()
    {
        board.FullBoard -= HandleGameOver;
    }

    // ==================================================
    // Initialize/Exit
    // ==================================================

    public void Initialize(UIData data, Board board, Tutorial tutorial)
    {
        Debug.Assert(!instantiated, "Instance of UIManager already exists.");
        instantiated = true;

        this.data = data;
        this.board = board;
        this.tutorial = tutorial;

        baseViewController = new ViewController<BaseView, BaseViewType, IUIData>(baseViewList, BaseViewCapacity, this);
        popUpViewController = new ViewController<PopUpView, PopUpViewType, IUIData>(popUpViewList, PopUpViewCapacity, this);
    }

    public UIData Exit()
    {
        return data;
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
        if (playSound && data.Settings.SFXOn)
        {
            ServiceLocator.Get<IAudio>().PlaySoundEffect(AudioType.Button);
            ServiceLocator.Get<IAudio>().PlaySoundEffect(AudioType.Transition);
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
        
        baseViewController.PushView(type, data.Profile);
    }

    // rename to push pop up view
    private void PushOverlay(PopUpViewType type, bool playSound = true)
    {
        if (playSound && data.Settings.SFXOn)
        {
            ServiceLocator.Get<IAudio>().PlaySoundEffect(AudioType.Button);
        }

        IUIData viewData = data.Settings;

        if (type == PopUpViewType.Pause)
        {
            board.PauseGame(true);
        }
        else if (type == PopUpViewType.Profile || type == PopUpViewType.ChooseAvatar)
        {
            viewData = data.Profile;
        }

        popUpViewController.PushView(type, viewData);
    }

    // rename to pop pop up view
    private void PopOverlay()
    {
        if (data.Settings.SFXOn)
            ServiceLocator.Get<IAudio>().PlaySoundEffect(AudioType.Button);

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
        data.Settings = newSettings;
    }

    // note: this NEVER updated the score list
    private void UpdateProfile(ProfileData newProfile)
    {
        data.Profile.Avatar = newProfile.Avatar;
        data.Profile.Username = newProfile.Username;

        baseViewController.UpdateView(BaseViewType.Home, data.Profile);
        popUpViewController.UpdateView(PopUpViewType.Profile, data.Profile);
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    private void HandleGameOver(int score, int highScore)
    {
        FinalScoreData scoreData = new FinalScoreData(score, highScore);
        baseViewController.PushView(BaseViewType.GameOver, scoreData);

        data.Profile.ScoreList.TryAddValue(score);
    }


}
