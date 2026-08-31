using UnityEngine;
using System;
using System.Collections;
//using System.Threading.Tasks; --- not sure if i want to use this or not

// todo:
    // try/catch for init/load gameplay
        // move each into their own function
        // create the try-catch statement
        // implement what to do on fail
        // go through and add load exceptions to each scricpt for fatal load/init
    // consider using threads for loading

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
    private DiscStorage discService = new DiscStorage();
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
            LoadData();
            InitServices();
            InitSystems();
        }
        catch (Exception e)
        {
            // implement what to do on an exception
        }

        // loading done
        StartGame();
    }

    // only runs once everything is done being loaded
    /*
    private void Start()
    {
        // run load sequence
        StartCoroutine(LoadSequence());
    }*/

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

    // turn this into a task
    // first: separate out sub tasks into their own functions

    // task #1: load data
    private void LoadData()
    {
        hasLaunched = PlayerPrefsStorage.GetBool(InitKeys.HasLaunched, false);
        inProgress = PlayerPrefsStorage.GetBool(InitKeys.InProgress, false);

        if (inProgress)
        {
            gameData = discService.Load<GameData>(DataFiles.GameData);
            if (gameData == null)
            {
                Debug.LogError("Failed to load GameData — falling back to new game.");
                gameData = new GameData();
            }
        }
        else
            gameData = new GameData();

        uIData = discService.Load<UIData>(DataFiles.UIData);
        if (uIData == null)
            uIData = new UIData();
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
            PlayerPrefsStorage.SetBool(InitKeys.HasLaunched, true);
        }

        // open the scene
        uIManager.PushView<BaseViewType>(startScreen);
        audioService.PlayMusic(AudioType.UIMusic);

        active = true;  // also means load complete in this instance
    }

    /*
    private IEnumerator LoadSequence()
    {
        // show the load screen
        loadScreen.SetActive(true);
        yield return null;

        // load data
        GameData gameData;
        if (inProgress)
        {
            gameData = discService.Load<GameData>(DataFiles.GameData);
            if (gameData == null)
            {
                Debug.LogError("Failed to load GameData — falling back to new game.");
                gameData = new GameData();
            }
        }
        else
            gameData = new GameData();
        yield return null;

        UIData uIData = discService.Load<UIData>(DataFiles.UIData);
        if (uIData == null)
            uIData = new UIData();
        yield return null;

        // replace these with safety checks
        Debug.Assert(gameData != null);
        Debug.Assert(uIData != null);
        Debug.Assert(uIData.Profile.ScoreList != null);
        Debug.Assert(board != null);

        // scaling
        Scaler.CalculateAndSetScale(Camera.main);
        Scaler.ApplyLocalScale(backgroundTransform);
        yield return null;

        // load resources/inject services
        audioService = new AudioService(uIData.AudioSettings, musicSource, sFXSource);
        ServiceLocator.Register<IAudio>(audioService);

        vibrationService = new VibrationService(uIData.VibrationOn);
        ServiceLocator.Register<IVibration>(vibrationService);

        spriteDatabase = Resources.Load<SpriteDatabase>("SpriteDatabase");
        spriteDatabase.Initialize();
        ServiceLocator.Register<ISpriteDatabase>(spriteDatabase);
        yield return null;

        // initialize systems
        // put try/catch here for init and then load

        board.Initialize(uIData.Profile.ScoreList.HighScore());
        yield return null;

        if (!hasLaunched)
        {
            board.RunTutorial();
            tutorial.Initialize(board);
        }
        else if (inProgress)
            board.Load(gameData);
            
        yield return null;

        uIManager.Initialize(uIData.Profile, board, tutorial);
        yield return null;

        // Load Complete
        Debug.Log("load complete");
        loadScreen.SetActive(false);

        // set the opening view
        BaseViewType startScreen = BaseViewType.Home;
        if (!hasLaunched)
        {
            Debug.Log("launch tutorial");

            startScreen = BaseViewType.Tutorial;
            PlayerPrefsStorage.SetBool(InitKeys.HasLaunched, true);
        }

        // open the scene
        uIManager.PushView<BaseViewType>(startScreen);
        audioService.PlayMusic(AudioType.UIMusic);

        active = true;  // also means load complete in this instance
    }*/

    private void ExitAndSave()
    {
        if (!active) return;

        // player prefs save
        Debug.Log($"in progress: {board.InProgress}");
        PlayerPrefsStorage.SetBool(InitKeys.InProgress, board.InProgress);

        // exit systems
        UIData uIData = new UIData();
        uIData.Profile = uIManager.Exit();
        uIData.AudioSettings = audioService.GetSettings();
        uIData.VibrationOn = vibrationService.GetSettings();

        GameData gameData = board.Exit();

        // disc save
        discService.Save<UIData>(DataFiles.UIData, uIData);
        if (board.InProgress)
            discService.Save<GameData>(DataFiles.GameData, gameData);

        active = false;
    }

    private void Reenter()
    {
        if (active) return;

        active = true;
    }

}
