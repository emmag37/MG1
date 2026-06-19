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
    GameState State { get; }
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

public interface IAllData : IRuntimeData
{
    IGameData GameData { get; }
    IUserSettings UserSettings { get; }
}

// does not need to be runtime data, never passed to the UI on its own
public interface IGamePlayData
{
    CellColor CurrentPlayer { get; }
    CellColor NextPlayer { get; }
    int CurrentScore { get; }
    BoardData Board { get; }
}

// ==================================================
// Classes
// ==================================================

public class GameData : IGameData
{
    public GameState State { get; set; }
    public int Score { get; set; }
    public int HighScore { get; set; }
    public IReadOnlyList<int> ScoreHistory { get; set; }
    public IReadOnlyList<LeaderboardData> LeaderboardRanking { get; set; }

    public GameData(GameState state, int score, int highScore, IReadOnlyList<int> scoreHistory, IReadOnlyList<LeaderboardData> leaderboardRanking)
    {
        State = state;
        Score = score;
        HighScore = highScore;
        ScoreHistory = scoreHistory;
        LeaderboardRanking = leaderboardRanking;
    }
}

public class GamePlayData : IGamePlayData
{
    public CellColor CurrentPlayer { get; set; }
    public CellColor NextPlayer { get; set; }
    public int CurrentScore { get; set; }
    public BoardData Board { get; set; }

    public GamePlayData(CellColor currentPlayer, CellColor nextPlayer, int currentScore, BoardData board)
    {
        CurrentPlayer = currentPlayer;
        NextPlayer = nextPlayer;
        CurrentScore = currentScore;
        Board = board;
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

public class AllData : IAllData
{
    public IGameData GameData { get; set; }
    public IUserSettings UserSettings { get; set; }

    public AllData(IGameData gameData, IUserSettings userSettings)
    {
        GameData = gameData;
        UserSettings = userSettings;
    }
}

