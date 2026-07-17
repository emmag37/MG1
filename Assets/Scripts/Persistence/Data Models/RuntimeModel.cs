using UnityEngine;
using System;
using System.Collections.Generic;


// ==================================================
// Interfaces
// ==================================================

public interface IGameData : IUIData
{
    bool InProgress { get; }
    int Score { get; }
    int HighScore { get; }

    IReadOnlyList<int> ScoreHistory { get; }    // might put this in its own runtime data type, keep for now
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
    public bool InProgress { get; set; }
    public int Score { get; set; }
    public int HighScore { get; set; }
    public IReadOnlyList<int> ScoreHistory { get; set; }

    public GameData(bool inProgress, int score, int highScore, IReadOnlyList<int> scoreHistory)
    {
        InProgress = inProgress;
        Score = score;
        HighScore = highScore;
        ScoreHistory = scoreHistory;
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

