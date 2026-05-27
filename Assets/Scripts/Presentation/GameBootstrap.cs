using UnityEngine;

// move this to application?
public class GameBootstrap : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private BaseViewController baseViewController;
    [SerializeField] private PopUpViewController popUpViewController;
    [SerializeField] private TutorialViewController tutorialViewController;

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

        // initialize systems
        IUserSettings userSettings = settingsService.GetSettings();

        uiManager.Initialize(settingsService, gameDataService, gameManager);
        audioManager.Initialize(userSettings.MusicOn, userSettings.SFXOn);
        gameManager.Initialize(gameDataService);

        baseViewController.Initialize();
        popUpViewController.Initialize();

        // wire dependencies
        WireSettings();
        WireUI();
    }

    void Start()
    {
        uiManager.ShowView(BaseViewType.Home, false);
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

        settingsService.ProfileUpdate += baseViewController.HandleProfileUpdate;
        settingsService.ProfileUpdate += popUpViewController.HandleProfileUpdate;
    }

    private void UnwireSettings()
    {
        settingsService.MusicUpdate -= audioManager.HandleMusicUpdate;
        settingsService.SFXUpdate -= audioManager.HandleSFXUpdate;

        settingsService.ProfileUpdate -= baseViewController.HandleProfileUpdate;
        settingsService.ProfileUpdate -= popUpViewController.HandleProfileUpdate;
    }

    private void WireUI()
    {
        uiManager.ButtonPressed += audioManager.HandleButtonPressed;

        uiManager.ShowBaseView += baseViewController.HandleShowView;
        uiManager.PushOverlayView += popUpViewController.HandlePush;
        uiManager.PopOverlayView += popUpViewController.HandlePop;

        uiManager.SkipTutorial += tutorialViewController.HandleSkipTutorial;
    }

    private void UnwireUI()
    {
        uiManager.ButtonPressed -= audioManager.HandleButtonPressed;

        uiManager.ShowBaseView -= baseViewController.HandleShowView;
        uiManager.PushOverlayView -= popUpViewController.HandlePush;
        uiManager.PopOverlayView -= popUpViewController.HandlePop;

        uiManager.SkipTutorial -= tutorialViewController.HandleSkipTutorial;
    }
}
