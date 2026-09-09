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
    private bool instantiated = false;  // enfore single instantiation

    private IAudio audioService;

    private ProfileData profile;        // this should be the ONLY instance of profile (outside of load in bootstrap)

    private Board board;
    private Tutorial tutorial;

    private ViewController<BaseView, BaseViewType, IUIData> baseViewController;
    private ViewController<PopUpView, PopUpViewType, IUIData> popUpViewController;


    // ================================
    // Unity Lifecycle
    // ================================

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

        if (profile == null)
            throw new ArgumentNullException(nameof(profile));
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        this.profile = profile;
        this.board = board;
        this.tutorial = tutorial;

        baseViewController = new ViewController<BaseView, BaseViewType, IUIData>(baseViewList, BaseViewCapacity, this);
        popUpViewController = new ViewController<PopUpView, PopUpViewType, IUIData>(popUpViewList, PopUpViewCapacity, this);

        audioService = ServiceLocator.Get<IAudio>();

        board.FullBoard += HandleGameOver;

        instantiated = true;
    }

    // done
    public ProfileData Exit()
    {
        if (profile == null)
            throw new InvalidOperationException("[UIManager] Null profile during Exit");

        return profile;
    }

    // ==================================================
    // Interface Method Delegation
    // ==================================================

    public void PushView<TType>(TType type) where TType : struct, Enum
    {
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

    public void PatchUpdate(IUIPatch patch)
    {
        switch (patch)
        {
            case AvatarPatch p:
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

    // add error checking
    private void ShowBaseView(BaseViewType type)
    {
        ClearOverlay();

        // update reliant game/tutorial states and sound effects
        if (type == BaseViewType.GamePlay && baseViewController.PeekViewType() == BaseViewType.GamePlay)
        {
            board.PlayGame(restart: true);
        }
        else if (type == BaseViewType.GamePlay)
        {
            audioService.PlaySoundEffect(AudioType.Transition);
            board.PlayGame();
        } else if (type == BaseViewType.Tutorial && baseViewController.PeekViewType() == BaseViewType.Tutorial)
        {
            tutorial.SkipTutorial();
            return;
        }
        else if (type == BaseViewType.Tutorial)
        {
            tutorial.StartTutorial();
        }
        else if (type == BaseViewType.Home)
        {
            board.PauseGame(true);
            if (baseViewController.PeekViewType() != BaseViewType.None)
                audioService.PlaySoundEffect(AudioType.Transition);
        }

        // choose music
        if (type == BaseViewType.GamePlay)
            audioService.PlayMusic(AudioType.GameMusic);
        else
            audioService.PlayMusic(AudioType.UIMusic);
        
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
            Debug.Log("pop pause");
            board.PauseGame(false);
        }

        popUpViewController.PopView();
    }

    private void ClearOverlay()
    {
        while (popUpViewController.Count > 0)
        {
            PopOverlay();
        }
    }

    // ==================================================
    // Patch Methods
    // ==================================================

    private void AvatarUpdate(CellColor avatar)
    {
        if (profile.Avatar == avatar) return;

        profile.Avatar = avatar;
        baseViewController.UpdateView(BaseViewType.Home, profile);
        popUpViewController.UpdateView(PopUpViewType.Profile, profile);
    }

    private void UsernameUpdate(string username)
    {
        profile.Username = username;
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
