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

    // ==================================================
    // Private Fields
    // ==================================================
    private SettingsService settingsService;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        // create services
        settingsService = new SettingsService();
        IUserSettings userSettings = settingsService.GetSettings();

        // initialize systems
        uiManager.Initialize(settingsService, gameManager, userSettings.HasLaunched);
        audioManager.Initialize(userSettings.MusicOn, userSettings.SFXOn);
        gameManager.Initialize();

        baseViewController.Initialize();
        popUpViewController.Initialize();
        tutorialController.Initialize();

        // wire dependencies
        WireSettings();
        WireUI();
    }

    void Start()
    {
        uiManager.ShowView(BaseViewType.Home, false);

        IUserSettings userSettings = settingsService.GetSettings();
        if (!userSettings.HasLaunched)
        {
            uiManager.PushOverlay(PopUpViewType.Tutorial);
            settingsService.SetLaunched(true);
        }

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
