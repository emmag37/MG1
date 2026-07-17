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

    private SettingsService settingsService;
    private GameDataService gameDataService;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        // create services
        playerPrefs = new PlayerPrefsStorage();
        disc = new DiscStorage();

        settingsService = new SettingsService(playerPrefs);
        gameDataService = new GameDataService(disc, playerPrefs);

        IUserSettings userSettings = settingsService.GetSettings();

        // set the init flag
        bool hasLaunched = userSettings.HasLaunched;
        bool inProgress = gameDataService.GetGameData().InProgress;
        InitFlag initInfo = (hasLaunched ? 0 : InitFlag.Tutorial) | (inProgress ? InitFlag.LoadGame : 0);
        Debug.Log($"Init info: {initInfo}");

        board.Initialize(initInfo, gameDataService);
        tutorial.Initialize(board);

        uiManager.Initialize(initInfo, board, tutorial, settingsService);
        audioManager.Initialize(userSettings.MusicOn, userSettings.SFXOn);

        // wire dependencies
        WireSettings();
        WireUI();
    }

    void Start()
    {

        // here is where you need to put the load in info
        // if tutorial, prepare the tutorial sequence
        // if active game, load in the game

        BaseViewType startScreen = BaseViewType.Home;
        if (!(settingsService.GetSettings().HasLaunched))
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
        UnwireSettings();
        UnwireUI();
    }


    // ==================================================
    // Wire Methods
    // ==================================================

    private void WireSettings()
    {
        settingsService.MusicUpdate += audioManager.HandleMusicUpdate;
        settingsService.SFXUpdate += audioManager.HandleSFXUpdate;

        settingsService.ProfileUpdate += uiManager.HandleProfileUpdate;
    }

    private void UnwireSettings()
    {
        settingsService.MusicUpdate -= audioManager.HandleMusicUpdate;
        settingsService.SFXUpdate -= audioManager.HandleSFXUpdate;

        settingsService.ProfileUpdate += uiManager.HandleProfileUpdate;
    }

    private void WireUI()
    {
        uiManager.ButtonPressed += audioManager.HandleButtonPressed;
        uiManager.Transition += audioManager.HandleTransition;
    }

    private void UnwireUI()
    {
        uiManager.ButtonPressed -= audioManager.HandleButtonPressed;
        uiManager.Transition -= audioManager.HandleTransition;
    }


}
