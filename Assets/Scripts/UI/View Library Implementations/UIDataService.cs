using UnityEngine;

// todo: create a new username class to update its behavior, like capped ranked list
    // make only the string public/serializable

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

    public void Initialize(DiscStorage discStorageUtility)
    {
        this.discStorageUtility = discStorageUtility;

        LoadData();
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void AddScoreToList(int score)
    {
        Profile.ScoreList.TryAddValue(score);
    }

    // ==================================================
    // Load/Save
    // ==================================================

    private void LoadData()
    {
        data = discStorageUtility.Load<UIData>(DataFiles.UIData);

        Settings = data.SettingsData;
        Profile = data.ProfileData;
    }

    private void SaveData()
    {
        data.SettingsData = Settings;
        data.ProfileData = Profile;

        discStorageUtility.Save<UIData>(DataFiles.UIData, data);
    }
    
    // save data
    private void OnApplicationPause(bool pauseStatus)
    {
        // app is being backgrounded
        if (pauseStatus) SaveData();
    }

    // save data
    private void OnApplicationFocus(bool hasFocus)
    {
        // app lost focus (backgrounded on some platforms, alt-tabbed on desktop)
        if (!hasFocus) SaveData();
    }
}