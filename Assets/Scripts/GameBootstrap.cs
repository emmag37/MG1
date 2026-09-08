using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;


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

    private UIData loadUIData;
    private GameData loadGameData;

    private bool active;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    // done
    private async void Awake()
    {
        loadScreen.SetActive(true);

        try
        {
            await LoadData();

            InitServices();
            InitSystems();

            StartGame();
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameBootstrap] Fatal exception {e} during load/init");
            Application.Quit();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // app is being backgrounded
        if (pauseStatus)
        {
            ExitAndSave();  // ???
        }
        else
            Reenter();      // ???
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // app lost focus (backgrounded on some platforms, alt-tabbed on desktop)
        if (!hasFocus)
        {
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

    // task #1: load data
    private async Task LoadData()
    {
        fileService = new JsonFileStorage();

        loadUIData = await fileService.LoadWithRetries<UIData>(DataFiles.UIData);
        hasLaunched = loadUIData.HasLaunched;
        inProgress = loadUIData.InProgress;

        if (inProgress)
            loadGameData = await fileService.LoadWithRetries<GameData>(DataFiles.GameData);
    }

    // task #2: initialize services
    private void InitServices()
    {
        audioService = new AudioService(loadUIData.AudioSettings, musicSource, sFXSource);  
        ServiceLocator.Register<IAudio>(audioService);                                  

        vibrationService = new VibrationService(loadUIData.VibrationOn);                    
        ServiceLocator.Register<IVibration>(vibrationService);                          

        spriteDatabase = Resources.Load<SpriteDatabase>("SpriteDatabase");              
        if (spriteDatabase == null)
            throw new InvalidOperationException("[GameBootstrap] No SpriteDatabase found under Resources/SpriteDatabase");

        spriteDatabase.Initialize();                                                    
        ServiceLocator.Register<ISpriteDatabase>(spriteDatabase);                       
    }

    // task #3: calculations and initialize system - REQUIRES services/data
    private void InitSystems()
    {
        // scaling
        Scaler.CalculateAndSetScale(Camera.main);
        Scaler.ApplyLocalScale(backgroundTransform);

        board.Initialize(loadUIData.Profile.ScoreList.HighScore());
        if (!hasLaunched)
        {
            board.RunTutorial();
            tutorial.Initialize(board);
        }
        else if (inProgress)
        {
            try
            {
                board.Load(loadGameData);           // can throw a recoverable exception (need to figure out the recovery logic)
            }
            catch (GameLoadException)
            {
                fileService.BackupCorruptData(DataFiles.GameData);
                Debug.LogError("[GameBootstrap] Failed to load saved game");
            }
        }

        uIManager.Initialize(loadUIData.Profile, board, tutorial);
    }

    // task #4: open scene and start game
    private void StartGame()
    {
        loadScreen.SetActive(false);

        // set the opening view
        BaseViewType startScreen = BaseViewType.Home;
        if (!hasLaunched)
        {
            startScreen = BaseViewType.Tutorial;
            hasLaunched = true;
        }

        // open the scene
        uIManager.PushView<BaseViewType>(startScreen);

        active = true;  // also means load complete in this instance
    }

    private void ExitAndSave()
    {
        if (!active) return;

        // prepare all data
        UIData exitUIData = new UIData();
        GameData exitGameData = null;
        try
        {
            exitUIData.Profile = uIManager.Exit();
            exitUIData.AudioSettings = audioService.GetSettings();
            exitUIData.VibrationOn = vibrationService.GetSettings();
        }
        catch (Exception e)     // known: invalid operation
        {
            Debug.LogError($"[GameBootstrap] Caught exception {e} while accessing UIData, saving loaded data instead");
            exitUIData = loadUIData;
        }
        exitUIData.InProgress = board.InProgress;       // must always be consistent w/ board, safe call
        exitUIData.HasLaunched = hasLaunched;

        if (exitUIData.InProgress)
        {
            try
            {
                exitGameData = board.GetGameData();
            }
            catch (Exception e)     // known: invalid operation, argument out of range
            {
                Debug.LogError($"[GameBootstrap] Caught exception {e} while accessing GameData, no game save");
                exitUIData.InProgress = false;
            }
        }

        // disc save
        fileService.Save<UIData>(DataFiles.UIData, exitUIData);     // check
        if (exitUIData.InProgress)
            fileService.Save<GameData>(DataFiles.GameData, exitGameData);       // check

        active = false;
    }

    private void Reenter()
    {
        if (active) return;

        active = true;
    }

}
