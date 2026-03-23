using UnityEngine;

public static class GamePrefs
{
    private const string HasLaunchedKey = "HasLaunchedBefore";
    private const string HighScoreKey = "HighScore";

    private const string UsernameKey = "Username";

    // First launch
    public static bool HasLaunchedBefore
    {
        get => PlayerPrefs.GetInt(HasLaunchedKey, 0) == 1;
        set => PlayerPrefs.SetInt(HasLaunchedKey, value ? 1 : 0);
    }

    // High score
    public static int HighScore
    {
        get => PlayerPrefs.GetInt(HighScoreKey, 0);
        set => PlayerPrefs.SetInt(HighScoreKey, value);
    }

    // Username
    public static string Username
    {
        get => PlayerPrefs.GetString(UsernameKey, "default");
        set => PlayerPrefs.SetString(UsernameKey, value);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
