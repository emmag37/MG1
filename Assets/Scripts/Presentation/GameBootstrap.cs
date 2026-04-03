using UnityEngine;

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
    [SerializeField] private TutorialController tutorialController;

    [SerializeField] private HUDController hUDController;

    // ==================================================
    // Private Fields
    // ==================================================
    private PlayerPrefsStorage storage;
    private SettingsService settingsService;
    private GameDataService gameDataService;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        // create services
        storage = new PlayerPrefsStorage();
        settingsService = new SettingsService(storage);
        gameDataService = new GameDataService(storage);

        // initialize systems
        IUserSettings userSettings = settingsService.GetSettings();

        uiManager.Initialize(settingsService, gameManager, userSettings.HasLaunched);
        audioManager.Initialize(userSettings.MusicOn, userSettings.SFXOn);
        gameManager.Initialize(gameDataService);

        baseViewController.Initialize();
        popUpViewController.Initialize();
        tutorialController.Initialize();

        // wire dependencies
        WireSettings();
        WireGameData();
        WireUI();
    }

    void Start()
    {
        uiManager.ShowView(BaseViewType.Home, false);

        IUserSettings userSettings = settingsService.GetSettings();
        if (!userSettings.HasLaunched)
        {
            uiManager.PushOverlay(PopUpViewType.Tutorial, false);
            settingsService.SetLaunched(true);
        }

        audioManager.Play();
    }

    void OnDestroy()
    {
        UnwireSettings();
        UnwireGameData();
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

    private void WireGameData()
    {
        gameDataService.NewScore += hUDController.HandleScoreUpdate;
    }

    private void UnwireGameData()
    {
        gameDataService.NewScore -= hUDController.HandleScoreUpdate;
    }

    private void WireUI()
    {
        uiManager.ButtonPressed += audioManager.HandleButtonPressed;

        uiManager.ShowBaseView += baseViewController.HandleShowView;
        uiManager.PushOverlayView += popUpViewController.HandlePush;
        uiManager.PopOverlayView += popUpViewController.HandlePop;

        uiManager.SwitchTutorialView += tutorialController.HandleSwitchView;
        uiManager.CloseTutorialView += tutorialController.HandleClose;
    }

    private void UnwireUI()
    {
        uiManager.ButtonPressed -= audioManager.HandleButtonPressed;

        uiManager.ShowBaseView -= baseViewController.HandleShowView;
        uiManager.PushOverlayView -= popUpViewController.HandlePush;
        uiManager.PopOverlayView -= popUpViewController.HandlePop;

        uiManager.SwitchTutorialView -= tutorialController.HandleSwitchView;
        uiManager.CloseTutorialView -= tutorialController.HandleClose;
    }
}
