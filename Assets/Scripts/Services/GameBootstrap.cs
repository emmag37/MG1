using UnityEngine;
using System;

[Flags]
public enum InitFlag : byte
{
    None =  0,              // initialize nothing besides normal systems
    Tutorial = 1 << 0,      // initialize the tutorial
    LoadGame = 1 << 1       // initialize a previously started game

    // can have 6 more flags
}


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
    private PlayerPrefsStorage playerPrefs;
    private DiscStorage disc;

    private GameDataService gameDataService;
    private UIDataService uIDataService;

    bool hasLaunched;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        // create services
        playerPrefs = new PlayerPrefsStorage();
        disc = new DiscStorage();

        gameDataService = new GameDataService(disc, playerPrefs);
        uIDataService = new UIDataService(disc);    // load in the UI data

        // set the init flag - turn these bools into player prefs, they don't need to be saved together
        // lets turn has launched into just player pref
        hasLaunched = playerPrefs.GetBool(InitKeys.HasLaunched, false);
        bool inProgress = gameDataService.GetGameData().InProgress;

        InitFlag initInfo = (hasLaunched ? 0 : InitFlag.Tutorial) | (inProgress ? InitFlag.LoadGame : 0);
        Debug.Log($"Init info: {initInfo}");

        board.Initialize(initInfo, gameDataService);
        tutorial.Initialize(board);

        uiManager.Initialize(board, tutorial, uIDataService);
        audioManager.Initialize(uIDataService.Settings.MusicOn, uIDataService.Settings.SFXOn);

        // wire dependencies
        WireUI();
    }

    void Start()
    {

        // here is where you need to put the load in info
        // if tutorial, prepare the tutorial sequence
        // if active game, load in the game

        BaseViewType startScreen = BaseViewType.Home;
        if (!hasLaunched)
        {
            Debug.Log("start tutorial");
            startScreen = BaseViewType.Tutorial;
        }

        // always open a fresh new game with the home view
        uiManager.PushView<BaseViewType>(startScreen);
        audioManager.Play();
    }

    void OnDestroy()
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
