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

    private ProfileData profile;

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

    public void Initialize(ProfileData profile, Board board, Tutorial tutorial)
    {
        Debug.Assert(!instantiated, "Instance of UIManager already exists.");
        instantiated = true;

        this.profile = profile;
        this.board = board;
        this.tutorial = tutorial;

        baseViewController = new ViewController<BaseView, BaseViewType, IUIData>(baseViewList, BaseViewCapacity, this, profile);
        popUpViewController = new ViewController<PopUpView, PopUpViewType, IUIData>(popUpViewList, PopUpViewCapacity, this, profile);

        audioService = ServiceLocator.Get<IAudio>();
    }

    public ProfileData Exit()
    {
        return profile;
    }

    // ==================================================
    // Interface Method Delegation
    // ==================================================

    public void PushView<TType>(TType type) where TType : struct, Enum
    {
        /*
        if (typeof(TType) == typeof(BaseViewType))
            ShowBaseView((BaseViewType)(object)type);
        else if (typeof(TType) == typeof(PopUpViewType))
            PushOverlay((PopUpViewType)(object)type);
        else
            Debug.LogError($"Unsupported view type: {typeof(TType).Name}");
        */

        switch (type)
        {
            case BaseViewType t:
                ShowBaseView(t);
                break;
            case PopUpViewType t:
                PushOverlay(t);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), $"Unhandled view type: {type.GetType()}");
        }
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
            UpdateProfile();
        else
            Debug.LogError($"Unsupported ui data type: {typeof(IUIData).Name}");
    }

    public void PatchUpdate(IUIPatch patch)
    {
        switch (patch)
        {
            case AvatarPatch p:
                Debug.Log("avatar patch update");
                AvatarUpdate(p.avatar);
                break;
            case UsernamePatch p:
                UsernameUpdate(p.username);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(patch), $"Unhandled patch type: {patch.GetType()}");
        }
    }


    // ==================================================
    // View Controller Methods
    // ==================================================

    // rename to show base view
    private void ShowBaseView(BaseViewType type)
    {
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
        
        baseViewController.PushView(type, profile);
    }

    // rename to push pop up view
    private void PushOverlay(PopUpViewType type, bool playSound = true)
    {
        if (type == PopUpViewType.Pause)
        {
            board.PauseGame(true);
        }

        popUpViewController.PushView(type, profile);
    }

    // rename to pop pop up view
    private void PopOverlay()
    {
        if (popUpViewController.PeekViewType() == PopUpViewType.Pause)
        {
            board.PauseGame(false);
        }

        popUpViewController.PopView();
    }

    // ==================================================
    // Patch Methods
    // ==================================================

    private void AvatarUpdate(CellColor avatar)
    {
        Debug.Log("avatar update");

        profile.Avatar = avatar;

        baseViewController.UpdateView(BaseViewType.Home, profile);
        popUpViewController.UpdateView(PopUpViewType.Profile, profile);
    }

    private void UsernameUpdate(string username)
    {
        // to do
    }

    // note: this NEVER updates the score list
    private void UpdateProfile()
    {
        //baseViewController.UpdateView(BaseViewType.Home, profile);
        //popUpViewController.UpdateView(PopUpViewType.Profile, profile);
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    private void HandleGameOver(int score, int highScore)
    {
        FinalScoreData scoreData = new FinalScoreData(score, highScore);
        baseViewController.PushView(BaseViewType.GameOver, scoreData);

        profile.ScoreList.TryAddValue(score);
    }


}
