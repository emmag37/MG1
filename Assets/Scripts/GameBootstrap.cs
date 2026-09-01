using UnityEngine;
using System;
using System.Collections;
using System.IO;
//using System.Threading.Tasks; --- not sure if i want to use this or not

// todo:
    // try/catch for init/load gameplay
        // write exceptions for each step in load/init, maintain list
        // create error message ui - retry load, corrupt data error message
        // write switch statement for recoverable load errors, manage retry loop
        
    // consider using threads for loading
    // i'm also rethinking implementing my leaderboard
        // would need a report function for unkind usernames

// First in script execution order (set to -10)
public class GameBootstrap : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private GameObject loadScreen;

    [SerializeField] private UIManager uIManager;
    [SerializeField] private Board board;
    [SerializeField] private Tutorial tutorial;

    [SerializeField] private AudioSource sFXSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private Transform backgroundTransform;
 
    // ==================================================
    // Private Fields
    // ==================================================
    private JsonFileStorage fileService;
    private AudioService audioService;
    private VibrationService vibrationService;
    private SpriteDatabase spriteDatabase;

    private bool hasLaunched;
    private bool inProgress;

    private UIData uIData;
    private GameData gameData;

    private bool active;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    // synchronous for now, move to async
    private void Awake()
    {
        // show the load screen
        loadScreen.SetActive(true);

        // run the tasks - all must be run after one another
        try
        {
            LoadData();         // current work place
            InitServices();
            InitSystems();

            // loading done
            StartGame();
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameBootstrap] Load/Initialization failed with {e}");

            bool retry = RecoverableLoadException(e);

            if (RecoverableLoadException(e))
            {
                // give retry option
            }
            else
            {
                // alert that load failed
            }
        }
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

    // for testing in the editor - maybe keep for the build?
    private void OnApplicationQuit()
    {
        ExitAndSave();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    // check load exceptions
    private bool RecoverableLoadException(Exception e)
    {
        switch (e)
        {
            case FileNotFoundException:
                return true;
            case IOException:
                return true;
            case UnauthorizedAccessException:
                return true;
            default:
                return false;
        }
    }

    // task #1: load data
    private void LoadData()
    {
        fileService = new JsonFileStorage();

        uIData = fileService.Load<UIData>(DataFiles.UIData);
        hasLaunched = uIData.HasLaunched;
        inProgress = uIData.InProgress;

        if (inProgress)
            gameData = fileService.Load<GameData>(DataFiles.GameData);
    }

    // task #2: initialize services
    private void InitServices()
    {
        audioService = new AudioService(uIData.AudioSettings, musicSource, sFXSource);
        ServiceLocator.Register<IAudio>(audioService);

        vibrationService = new VibrationService(uIData.VibrationOn);
        ServiceLocator.Register<IVibration>(vibrationService);

        spriteDatabase = Resources.Load<SpriteDatabase>("SpriteDatabase");
        spriteDatabase.Initialize();
        ServiceLocator.Register<ISpriteDatabase>(spriteDatabase);
    }

    // task #3: calculations and initialize system - REQUIRES services/data
    private void InitSystems()
    {
        // scaling
        Scaler.CalculateAndSetScale(Camera.main);
        Scaler.ApplyLocalScale(backgroundTransform);

        board.Initialize(uIData.Profile.ScoreList.HighScore());
        if (!hasLaunched)
        {
            board.RunTutorial();
            tutorial.Initialize(board);
        }
        else if (inProgress)
            board.Load(gameData);

        uIManager.Initialize(uIData.Profile, board, tutorial);
    }

    // task #4: open scene and start game
    private void StartGame()
    {
        Debug.Log("load complete");
        loadScreen.SetActive(false);

        // set the opening view
        BaseViewType startScreen = BaseViewType.Home;
        if (!hasLaunched)
        {
            Debug.Log("launch tutorial");

            startScreen = BaseViewType.Tutorial;
            hasLaunched = true;
        }

        // open the scene
        uIManager.PushView<BaseViewType>(startScreen);
        audioService.PlayMusic(AudioType.UIMusic);

        active = true;  // also means load complete in this instance
    }

    private void ExitAndSave()
    {
        if (!active) return;

        Debug.Log($"in progress: {board.InProgress}");

        // prepare UIData
        UIData uIData = new UIData();
        uIData.HasLaunched = hasLaunched;
        uIData.InProgress = board.InProgress;
        uIData.Profile = uIManager.Exit();
        uIData.AudioSettings = audioService.GetSettings();
        uIData.VibrationOn = vibrationService.GetSettings();

        // exit systems
        GameData gameData = board.Exit();

        // disc save
        fileService.Save<UIData>(DataFiles.UIData, uIData);
        if (board.InProgress)
            fileService.Save<GameData>(DataFiles.GameData, gameData);

        active = false;
    }

    private void Reenter()
    {
        if (active) return;

        active = true;
    }

}
