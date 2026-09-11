using UnityEngine;


public static class PlayerPrefsStorage
{
    // all get methods - do they need to throw exceptions?

    // get/set bool - needs to be stored as an int
    public static bool GetBool(string key, bool defaultValue)
    {
        int value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
        return value == 1;
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        Save();
    }

    // get/set int
    public static int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        Save();
    }

    // get/set string
    public static string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        Save();
    }

    private static void Save()
    {
        PlayerPrefs.Save();
    }
}
