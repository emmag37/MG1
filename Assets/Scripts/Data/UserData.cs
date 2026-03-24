using UnityEngine;

public struct NoData { }

public class PlayerProfile
{
    private int highScore;
    private int recentScore;

    private string username;
    private int avatar;

    public int HighScore
    {
        get => highScore;
        set
        {
            highScore = value;
            PlayerPrefs.SetInt("HighScore", value);
        }
    }

    public int RecentScore
    {
        get => recentScore;
        set { recentScore = value; }
    }

    public string Username
    {
        get => username;
        set
        {
            username = value;
            PlayerPrefs.SetString("Username", value);
        }
    }

    public int Avatar
    {
        get => avatar;
        set
        {
            avatar = value;
            PlayerPrefs.SetInt("Avatar", value);
        }
    }

    // Load from storage
    public void Load()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        recentScore = PlayerPrefs.GetInt("RecentScore", 0);
        username = PlayerPrefs.GetString("Username", "");
        avatar = PlayerPrefs.GetInt("Avatar", 1);
    }
}

public class GameSettings
{
    private int hasLaunched;
    private int musicOn;
    private int effectsOn;

    public int HasLaunched
    {
        get => hasLaunched;
        set
        {
            hasLaunched = value;
            PlayerPrefs.SetInt("HasLaunched", value);
        }
    }

    public int MusicOn
    {
        get => musicOn;
        set
        {
            musicOn = value;
            PlayerPrefs.SetInt("MusicOn", value);
        }
    }

    public int EffectsOn
    {
        get => effectsOn;
        set
        {
            effectsOn = value;
            PlayerPrefs.SetInt("EffectsOn", value);
        }
    }

    public void Load()
    {
        hasLaunched = PlayerPrefs.GetInt("HasLaunched", 0);
        musicOn = PlayerPrefs.GetInt("MusicOn", 1);
        effectsOn = PlayerPrefs.GetInt("EffectsOn", 1);
    }
}
