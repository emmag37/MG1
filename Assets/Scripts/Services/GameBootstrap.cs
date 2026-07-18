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
        playerPrefs = new PlayerPrefsStorage();
        disc = new DiscStorage();

        // load data
        hasLaunched = playerPrefs.GetBool(InitKeys.HasLaunched, false);

        uIDataService = GetComponent<UIDataService>();
        uIDataService.Initialize(disc);    // automatically loads in the UI data

        gameDataService = GetComponent<GameDataService>();
        gameDataService.Initialize(disc, playerPrefs);   // only loads game data if a game was in progress

        // initialize scene components
        board.Initialize(gameDataService, !hasLaunched, uIDataService.GetHighScore());
        if (!hasLaunched) tutorial.Initialize(board);

        uiManager.Initialize(board, tutorial, uIDataService);

        // initialize services
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

            playerPrefs.SetBool(InitKeys.HasLaunched, true);
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
