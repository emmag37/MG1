using UnityEngine;
using System;
using System.Collections;


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

    bool hasLaunched;
    bool inProgress;

    bool active;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    private void Awake()
    {
        // load data
        hasLaunched = PlayerPrefsStorage.GetBool(InitKeys.HasLaunched, false);
        inProgress = PlayerPrefsStorage.GetBool(InitKeys.InProgress, false);
    }

    // only runs once everything is done being loaded
    private void Start()
    {
        // run load sequence
        StartCoroutine(LoadSequence());
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
        board.Initialize(gameData, uIData.Profile.ScoreList.HighScore(), !hasLaunched, inProgress);
        yield return null;

        if (!hasLaunched)
            tutorial.Initialize(board);
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
    }

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
