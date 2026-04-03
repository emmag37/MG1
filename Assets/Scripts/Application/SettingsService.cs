using UnityEngine;
using System;

// this class manages its own saves in player prefs for now.
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
    private PlayerPrefsStorage storage;
    private UserSettings settings;


    // ==================================================
    // Constructor/Initializer
    // ==================================================

    public SettingsService(PlayerPrefsStorage storage)
    {
        this.storage = storage;

        settings = Load();
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void SetLaunched(bool launched)
    {
        settings.HasLaunched = launched;
        storage.SetBool(SettingsKeys.Launched, true);
    }

    public void SetMusicOn(bool on)
    {
        settings.MusicOn = on;
        storage.SetBool(SettingsKeys.Music, on);

        MusicUpdate?.Invoke(on);
    }

    public void SetSFXOn(bool on)
    {
        settings.SFXOn = on;
        storage.SetBool(SettingsKeys.SFX, on);

        SFXUpdate?.Invoke(on);
    }

    public void SetUsername(string name)
    {
        settings.Username = name;
        storage.SetString(SettingsKeys.Username, name);
    }

    public void SetAvatar(CellColor color)
    {
        settings.Avatar = color;
        storage.SetInt(SettingsKeys.Avatar, (int)color);

        ProfileUpdate?.Invoke(GetSettings());
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private UserSettings Load()
    {
        UserSettings newSettings = new UserSettings(
            launched: storage.GetBool(SettingsKeys.Launched, false),
            musicOn: storage.GetBool(SettingsKeys.Music, true),
            sfxOn: storage.GetBool(SettingsKeys.SFX, true),
            username: storage.GetString(SettingsKeys.Username, "default-name"),
            avatar: (CellColor)storage.GetInt(SettingsKeys.Avatar, (int)CellColor.Color1)
        );

        return newSettings;
    }

}
