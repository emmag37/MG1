using UnityEngine;
using System;

public class SettingsService : MonoBehaviour
{
    // ==================================================
    // Public Fields
    // ==================================================
    public IUserSettings GetSettings() => settings;

    // ==================================================
    // Events
    // ==================================================
    public event Action<IUserSettings> AudioUpdate;
    public event Action<IUserSettings> ProfileUpdate;

    // ==================================================
    // Private Fields
    // ==================================================
    private UserSettings settings;


    // ==================================================
    // Unity Lifecycle - change to initializer
    // ==================================================

    void Awake()
    {
        // load - make persistent
        settings = new UserSettings(true, true, true, "default-name", CellColor.Color1);
    }


    // ==================================================
    // Public Methods
    // ==================================================

    public void SetLaunched(bool launched)
    {
        settings.HasLaunched = launched;
    }

    public void SetMusicOn(bool on)
    {
        settings.MusicOn = on;
        AudioUpdate?.Invoke(GetSettings());
    }

    public void SetSFXOn(bool on)
    {
        settings.SFXOn = on;
        AudioUpdate?.Invoke(GetSettings());
    }

    public void SetUsername(string name)
    {
        settings.Username = name;
    }

    public void SetAvatar(CellColor color)
    {
        settings.Avatar = color;
        ProfileUpdate?.Invoke(GetSettings());
    }

}
