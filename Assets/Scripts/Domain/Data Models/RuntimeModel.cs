using UnityEngine;
using System;
using System.Collections.Generic;

public struct NoData { }

// ==================================================
// Interfaces
// ==================================================

public interface IRuntimeData { }

public interface IGameData : IRuntimeData
{
    int Score { get; }
    int HighScore { get; }

    IReadOnlyList<int> ScoreHistory { get; }    // might put this in its own runtime data type, keep for now
    IReadOnlyList<LeaderboardData> LeaderboardRanking { get; }
}

public interface IUserSettings : IRuntimeData
{
    bool HasLaunched { get; }
    bool MusicOn { get; }
    bool SFXOn { get; }
    string Username { get; }
    CellColor Avatar { get; }
}

// ==================================================
// Classes
// ==================================================

public class GameData : IGameData
{
    public int Score { get; set; }
    public int HighScore { get; set; }
    public IReadOnlyList<int> ScoreHistory { get; set; }
    public IReadOnlyList<LeaderboardData> LeaderboardRanking { get; set; }

    public GameData(int score, int highScore, IReadOnlyList<int> scoreHistory, IReadOnlyList<LeaderboardData> leaderboardRanking)
    {
        Score = score;
        HighScore = highScore;
        ScoreHistory = scoreHistory;
        LeaderboardRanking = leaderboardRanking;
    }
}

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

