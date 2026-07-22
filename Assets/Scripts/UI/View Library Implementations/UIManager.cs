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
    // Inspector Fields
    // ==================================================
    [SerializeField] private BaseView[] baseViewList;
    [SerializeField] private PopUpView[] popUpViewList;


    // ==================================================
    // Private Fields
    // ==================================================
    private bool instantiated = false;

    private IAudio audioService;

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

        audioService = ServiceLocator.Get<IAudio>();
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
        if (data is ProfileData profile)
            UpdateProfile(profile);
        else
            Debug.LogError($"Unsupported ui data type: {typeof(IUIData).Name}");
    }


    // ==================================================
    // View Controller Methods
    // ==================================================

    // rename to show base view
    private void ShowBaseView(BaseViewType type)
    {
        if (baseViewController.PeekViewType() != BaseViewType.None)
            audioService.PlaySoundEffect(AudioType.Button);     // move this to the actual button

        if (popUpViewController.Count > 0) popUpViewController.ClearViews();

        if (type == BaseViewType.GamePlay && baseViewController.PeekViewType() == BaseViewType.GamePlay)
        {
            board.PlayGame(restart: true);
        }
        else if (type == BaseViewType.GamePlay)
        {
            audioService.PlaySoundEffect(AudioType.Transition);
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
        else if (type == BaseViewType.Home && baseViewController.PeekViewType() != BaseViewType.None)
        {
            audioService.PlaySoundEffect(AudioType.Transition);
        }
        
        baseViewController.PushView(type, data.Profile);
    }

    // rename to push pop up view
    private void PushOverlay(PopUpViewType type, bool playSound = true)
    {
        audioService.PlaySoundEffect(AudioType.Button);    // move to the button

        IUIData viewData = data.Profile;    // used to be data.Settings

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
        audioService.PlaySoundEffect(AudioType.Button);     // move to the button

        if (popUpViewController.PeekViewType() == PopUpViewType.Pause)
        {
            board.PauseGame(false);
        }

        popUpViewController.PopView();
    }

    // ==================================================
    // UI Data Methods
    // ==================================================

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
