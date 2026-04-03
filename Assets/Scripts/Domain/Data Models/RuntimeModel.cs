using UnityEngine;

public struct NoData { }

// read only
public interface IGameData
{
    int Score { get; }  // edit this to be final score maybe? idk this needs to compile
    int HighScore { get; }
}

// read and write
public class GameData : IGameData
{
    public int Score { get; set; }
    public int HighScore { get; set; }

    public GameData(int score, int highScore)
    {
        Score = score;
        HighScore = highScore;
    }
}

// read only
public interface IUserSettings
{
    bool HasLaunched { get; }
    bool MusicOn { get; }
    bool SFXOn { get; }
    string Username { get; }
    CellColor Avatar { get; }
}

// read and write
public class UserSettings : IUserSettings
{
    public bool HasLaunched { get; set; }
    public bool MusicOn { get; set; }
    public bool SFXOn { get; set; }
    public string Username { get; set; }
    public CellColor Avatar { get; set; }

    public UserSettings(bool launched, bool musicOn, bool sfxOn, string username, CellColor avatar)
    {
        HasLaunched = launched;
        MusicOn = musicOn;
        SFXOn = sfxOn;
        Username = username;
        Avatar = avatar;
    }
}

