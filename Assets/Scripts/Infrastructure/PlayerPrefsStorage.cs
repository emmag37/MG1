using UnityEngine;

public static class SettingsKeys
{
    public const string Launched = "game.launched";
    public const string Music = "audio.music";
    public const string SFX = "audio.sfx";
    public const string Username = "profile.username";
    public const string Avatar = "profile.avatar";
}

public static class GameDataKeys
{
    public const string HighScore = "game.highScore";
}

public class PlayerPrefsStorage
{
    // get/set bool - needs to be stored as an int
    public bool GetBool(string key, bool defaultValue)
    {
        int value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
        return value == 1;
    }

    public void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        Save();
    }

    // get/set int
    public int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        Save();
    }

    // get/set string
    public string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        Save();
    }

    private void Save()
    {
        PlayerPrefs.Save();
    }
}
