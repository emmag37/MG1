using UnityEngine;
using System;

// todo: put in progress in player prefs

// First in script execution order (set to -10)
public class GameBootstrap : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private UIManager uIManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Board board;
    [SerializeField] private Tutorial tutorial;

    // ==================================================
    // Private Fields
    // ==================================================
    private PlayerPrefsStorage playerPrefs = new PlayerPrefsStorage();
    private DiscStorage disc = new DiscStorage();

    bool hasLaunched;
    bool inProgress;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    private void Awake()
    {
        // load data
        hasLaunched = playerPrefs.GetBool(InitKeys.HasLaunched, false);
        inProgress = playerPrefs.GetBool(InitKeys.InProgress, false);

        GameData gameData;
        if (inProgress)
            gameData = disc.Load<GameData>(DataFiles.GameData);
        else
            gameData = new GameData();

        UIData uIData = disc.Load<UIData>(DataFiles.UIData);


        // initialize systems
        board.Initialize(gameData, uIData.Profile.ScoreList.HighScore(), hasLaunched, inProgress);
        if (!hasLaunched)
            tutorial.Initialize(board);

        uIManager.Initialize(uIData, board, tutorial);
        audioManager.Initialize(uIData.Settings.MusicOn, uIData.Settings.SFXOn);


        // wire dependencies
        WireUI();
    }

    private void Start()
    {

        // here is where you need to put the load in info
        // if tutorial, prepare the tutorial sequence
        // if active game, load in the game

        BaseViewType startScreen = BaseViewType.Home;
        if (!hasLaunched)
        {
            Debug.Log("start tutorial");
            startScreen = BaseViewType.Tutorial;

            playerPrefs.SetBool(InitKeys.HasLaunched, true);
        }

        // always open a fresh new game with the home view
        uIManager.PushView<BaseViewType>(startScreen);
        audioManager.Play();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // app is being backgrounded
        ExitAndSave();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // app lost focus (backgrounded on some platforms, alt-tabbed on desktop)
        ExitAndSave();
    }

    private void OnDestroy()
    {
        UnwireUI();
    }


    // ==================================================
    // Wire Methods
    // ==================================================

    private void WireUI()
    {
        uIManager.ButtonPressed += audioManager.HandleButtonPressed;
        uIManager.Transition += audioManager.HandleTransition;
        uIManager.SettingsUpdate += audioManager.HandleSettingsUpdate;
    }

    private void UnwireUI()
    {
        uIManager.ButtonPressed -= audioManager.HandleButtonPressed;
        uIManager.Transition -= audioManager.HandleTransition;
        uIManager.SettingsUpdate -= audioManager.HandleSettingsUpdate;
    }


    // ==================================================
    // Persistence Methods
    // ==================================================

    private void ExitAndSave()
    {
        // player prefs
        playerPrefs.SetBool(InitKeys.InProgress, board.InProgress);

        // disc
        UIData uIData = uIManager.Exit();
        disc.Save<UIData>(DataFiles.UIData, uIData);

        GameData gameData = board.Exit();
        if (board.InProgress)
            disc.Save<GameData>(DataFiles.GameData, gameData);
    }

}
