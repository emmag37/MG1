using UnityEngine;

public struct NoData { }

public struct UserScore
{
    public int Score;
    public int HighScore;
}

// for all presentation layer
public interface IUserSettings
{
    bool HasLaunched { get; }
    bool MusicOn { get; }
    bool SFXOn { get; }
    string Username { get; }
    CellColor Avatar { get; }
}

// for my settings service
public class UserSettings : IUserSettings
{
    public bool HasLaunched { get; set; }
    public bool MusicOn { get; set; }
    public bool SFXOn { get; set; }
    public string Username { get; set; }
    public CellColor Avatar { get; set; }

    public UserSettings(bool launched, bool musicOn, bool sfxOn, string username, CellColor color)
    {
        HasLaunched = launched;
        MusicOn = musicOn;
        SFXOn = sfxOn;
        Username = username;
        Avatar = color;
    }
}

