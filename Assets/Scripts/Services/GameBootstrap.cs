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

    [SerializeField] private AudioSource sFXSource;
    [SerializeField] private AudioSource musicSource;

    // ==================================================
    // Private Fields
    // ==================================================
    private DiscStorage disc = new DiscStorage();

    bool hasLaunched;
    bool inProgress;

    bool active = true;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    private void Awake()
    {
        // load data
        hasLaunched = PlayerPrefsStorage.GetBool(InitKeys.HasLaunched, false);
        inProgress = PlayerPrefsStorage.GetBool(InitKeys.InProgress, false);

        GameData gameData;
        if (inProgress)
            gameData = disc.Load<GameData>(DataFiles.GameData);
        else
            gameData = new GameData();

        UIData uIData = disc.Load<UIData>(DataFiles.UIData);    // is there an error here?

        Debug.Assert(gameData != null);
        Debug.Assert(uIData != null);
        Debug.Assert(uIData.Profile.ScoreList != null);     // null ref exception
        Debug.Assert(board != null);

        // initialize systems
        board.Initialize(gameData, uIData.Profile.ScoreList.HighScore(), !hasLaunched, inProgress);
        if (!hasLaunched)
            tutorial.Initialize(board);

        uIManager.Initialize(uIData, board, tutorial);
        audioManager.Initialize(uIData.Settings.MusicOn, uIData.Settings.SFXOn);

        // inject services
        ServiceLocator.Register<IAudio>(new AudioService(musicSource, sFXSource));

        // you can add the json file system later
        
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

            PlayerPrefsStorage.SetBool(InitKeys.HasLaunched, true);
        }

        // always open a fresh new game with the home view
        uIManager.PushView<BaseViewType>(startScreen);
        ServiceLocator.Get<IAudio>().PlayMusic(AudioType.UIMusic);

        //audioManager.Play();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // app is being backgrounded
        if (pauseStatus)
        {
            Debug.Log("pause application");
            ExitAndSave();
        }
        else
            Reenter();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // app lost focus (backgrounded on some platforms, alt-tabbed on desktop)
        if (!hasFocus)
        {
            Debug.Log("lose focus");
            ExitAndSave();
        }
        else
            Reenter();
    }

    // for testing in the editor
    private void OnApplicationQuit()
    {
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
        /*
        uIManager.ButtonPressed += audioManager.HandleButtonPressed;
        uIManager.Transition += audioManager.HandleTransition;
        uIManager.SettingsUpdate += audioManager.HandleSettingsUpdate;
        */
    }

    private void UnwireUI()
    {
        /*
        uIManager.ButtonPressed -= audioManager.HandleButtonPressed;
        uIManager.Transition -= audioManager.HandleTransition;
        uIManager.SettingsUpdate -= audioManager.HandleSettingsUpdate;
        */
    }


    // ==================================================
    // Lifecycle Management Methods
    // ==================================================

    private void ExitAndSave()
    {
        if (!active) return;

        // player prefs save
        Debug.Log($"in progress: {board.InProgress}");
        PlayerPrefsStorage.SetBool(InitKeys.InProgress, board.InProgress);

        // exit systems
        UIData uIData = uIManager.Exit();
        GameData gameData = board.Exit();

        // disc save
        disc.Save<UIData>(DataFiles.UIData, uIData);
        if (board.InProgress)
            disc.Save<GameData>(DataFiles.GameData, gameData);

        active = false;
    }

    private void Reenter()
    {
        if (active) return;

        active = true;
    }

}
