using UnityEngine;

// todo: implement load and save, implement add score to list

public class UIDataService : MonoBehaviour
{
    // ==================================================
    // Public Fields
    // ==================================================
    public SettingsData Settings;       // edit these and their properties directly
    public ProfileData Profile;

    // ==================================================
    // Private Fields
    // ==================================================
    private DiscStorage discStorageUtility;

    private UIData data;    // value that gets saved to json file

    // ==================================================
    // Constructor
    // ==================================================

    // for now, pass the json save/load system into constructor.
    // save/load is a good candidate for a service locator

    public UIDataService(DiscStorage discStorageUtility)
    {
        this.discStorageUtility = discStorageUtility;

        data = LoadData();
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void AddScoreToList(int score)
    {

    }

    // ==================================================
    // Load/Save
    // ==================================================

    private UIData LoadData()
    {
        // load the settings data

        // load the profile data

        // create data to return

        return null;
    }

    // save data
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // app is being backgrounded
            return;
    }

    // save data
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) // app lost focus (backgrounded on some platforms, alt-tabbed on desktop)
            return;
    }



}