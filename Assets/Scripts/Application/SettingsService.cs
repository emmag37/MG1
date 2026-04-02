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
    public event Action<bool> UpdatedMusicOn;
    public event Action<bool> UpdatedSFXOn;
    public event Action<string> UpdatedUsername;
    public event Action<CellColor> UpdatedAvatar;

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
        UpdatedMusicOn?.Invoke(on);
    }

    public void SetSFXOn(bool on)
    {
        settings.SFXOn = on;
        UpdatedSFXOn?.Invoke(on);
    }

    public void SetUsername(string name)
    {
        settings.Username = name;
        UpdatedUsername?.Invoke(name);
    }

    public void SetAvatar(CellColor color)
    {
        settings.Avatar = color;
        UpdatedAvatar?.Invoke(color);
    }

}
