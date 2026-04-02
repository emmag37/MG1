using UnityEngine;
using System;

public class SettingsService
{
    // ==================================================
    // Public Fields
    // ==================================================
    public IUserSettings GetSettings() => settings;

    // ==================================================
    // Events
    // ==================================================
    public event Action<bool> MusicUpdate;
    public event Action<bool> SFXUpdate;

    public event Action<IUserSettings> ProfileUpdate;

    // ==================================================
    // Private Fields
    // ==================================================
    private UserSettings settings;


    // ==================================================
    // Constructor/Initializer
    // ==================================================

    public SettingsService()
    {
        // load values once you add persistence

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
        MusicUpdate?.Invoke(on);
    }

    public void SetSFXOn(bool on)
    {
        settings.SFXOn = on;
        SFXUpdate?.Invoke(on);
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
