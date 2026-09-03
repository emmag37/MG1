using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

// todo:
    // new load screen w/ animation

    // then:
        // Exit and Save
        // Start Game
        
    // future features:
        // data validators so info passed to systems can be assumed safe
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

    private UIData loadUIData;
    private GameData loadGameData;

    private bool active;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    private async void Awake()
    {
        loadScreen.SetActive(true);
        await Task.Delay(1000);         // fake a longer load time to see animation

        try
        {
            await LoadDataWithRecovery(0);

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
    private void LoadData()
    {
        fileService = new JsonFileStorage();

        loadUIData = fileService.Load<UIData>(DataFiles.UIData);
        hasLaunched = loadUIData.HasLaunched;
        inProgress = loadUIData.InProgress;

        if (inProgress)
            loadGameData = fileService.Load<GameData>(DataFiles.GameData);
    }
    private async Task LoadDataWithRecovery(int attempt)
    {
        try
        {
            LoadData();
        }
        catch (FileNotFoundException)
        {
            throw new FileLoadException();
        }
        catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
        {
            Debug.LogError($"[GameBootstrap] Load data failed attempt {attempt} with {e}");
            int retries = e is UnauthorizedAccessException ? 2 : 3;
            if (attempt < retries)
            {
                // need to add delay
                await Task.Delay(100);
                await LoadDataWithRecovery(attempt + 1);
            }
            else
                throw new FileLoadException();      // for outer func to catch and kill program
        }
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
        audioService.PlayMusic(AudioType.UIMusic);

        active = true;  // also means load complete in this instance
    }

    private void ExitAndSave()
    {
        if (!active) return;

        // prepare UIData
        UIData exitUIData = new UIData();
        exitUIData.HasLaunched = hasLaunched;
        exitUIData.InProgress = board.InProgress;
        exitUIData.Profile = uIManager.Exit();
        exitUIData.AudioSettings = audioService.GetSettings();
        exitUIData.VibrationOn = vibrationService.GetSettings();

        // exit systems
        GameData exitGameData = board.Exit();
        
        // disc save
        fileService.Save<UIData>(DataFiles.UIData, exitUIData);
        if (board.InProgress)
            fileService.Save<GameData>(DataFiles.GameData, exitGameData);

        active = false;
    }

    private void Reenter()
    {
        if (active) return;

        active = true;
    }

}
