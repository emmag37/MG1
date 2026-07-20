using UnityEngine;
using System;

// todo: put in progress in player prefs

// First in script execution order (set to -10)
public class GameBootstrap : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private UIManager uiManager;
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
        board.Initialize(gameData, 0, hasLaunched, inProgress);
        if (!hasLaunched)
            tutorial.Initialize(board);

        uiManager.Initialize(uIData, board, tutorial);
        audioManager.Initialize(uIData.SettingsData.MusicOn, uIData.SettingsData.SFXOn);


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
        uiManager.PushView<BaseViewType>(startScreen);
        audioManager.Play();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // app is being backgrounded
        // save data
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // app lost focus (backgrounded on some platforms, alt-tabbed on desktop)
        // save data
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
        uiManager.ButtonPressed += audioManager.HandleButtonPressed;
        uiManager.Transition += audioManager.HandleTransition;
        uiManager.SettingsUpdate += audioManager.HandleSettingsUpdate;
    }

    private void UnwireUI()
    {
        uiManager.ButtonPressed -= audioManager.HandleButtonPressed;
        uiManager.Transition -= audioManager.HandleTransition;
        uiManager.SettingsUpdate -= audioManager.HandleSettingsUpdate;
    }


}
