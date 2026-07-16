using UnityEngine;
using System;
using System.Collections.Generic;

public class GameDataService
{
    // ==================================================
    // Public Fields
    // ==================================================
    public IGameData GetGameData() => data;
    public IGamePlayData GetGamePlayData() => game;

    // ==================================================
    // Private Fields
    // ==================================================
    private DiscStorage disc;
    private PlayerPrefsStorage playerPrefs;

    private GameData data;
    private GamePlayData game;


    // ==================================================
    // Constructor/Initializer
    // ==================================================

    public GameDataService(DiscStorage disc, PlayerPrefsStorage playerPrefs)
    {
        this.disc = disc;
        this.playerPrefs = playerPrefs;

        data = Load();
        game = LoadGame();
    }

    // ==================================================
    // Public Methods
    // ==================================================

    // game data methods
    public void SetInProgress(bool inProgress)
    {
        data.InProgress = inProgress;
        playerPrefs.SetBool(GameDataKeys.InProgress, inProgress);
    }

    public void UpdateHighScore(int highScore)
    {
        data.HighScore = highScore;
        playerPrefs.SetInt(GameDataKeys.HighScore, highScore);
    }

    public void SetFinalScore(int score)
    {
        data.Score = score;

        ScoreHistory history = disc.Load<ScoreHistory>(GameDataFiles.ScoreHistory);
        bool added = history.TryAddValue(score);
        disc.Save(GameDataFiles.ScoreHistory, history);

        if (added)
        {
            data.ScoreHistory = history.ROList;
        }
    }


    // game play data methods
    public void ResetGame()
    {
        // player colors don't need to be reset

        game.CurrentScore = 0;
        playerPrefs.SetInt(GameDataKeys.CurrentScore, 0);

        game.Board.Reset();
        disc.Save(GameDataFiles.BoardData, game.Board);
    }

    // saves the score and new cell on the grid
    public void SaveTurn(int newScore, Vector2Int index)
    {
        // update the score
        if (newScore > game.CurrentScore)
        {
            game.CurrentScore = newScore;
            playerPrefs.SetInt(GameDataKeys.CurrentScore, newScore);
        }

        // update and save the spot on the board
        game.Board.Set(index.x, index.y, (int)game.CurrentPlayer);
        disc.Save(GameDataFiles.BoardData, game.Board);
    }

    public void SavePlayerColors(CellColor currentColor, CellColor nextColor)
    {
        game.CurrentPlayer = currentColor;
        game.NextPlayer = nextColor;

        playerPrefs.SetInt(GameDataKeys.CurrentPlayer, (int)game.CurrentPlayer);
        playerPrefs.SetInt(GameDataKeys.NextPlayer, (int)game.NextPlayer);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private GameData Load()
    {
        GameData newData = new GameData(
            inProgress: playerPrefs.GetBool(GameDataKeys.InProgress, false),
            score: 0,
            highScore: playerPrefs.GetInt(GameDataKeys.HighScore, 0),
            scoreHistory: disc.Load<ScoreHistory>(GameDataFiles.ScoreHistory).ROList,
            leaderboardRanking: (new LeaderboardRanking()).ROList    // need to load from the cloud, empty list for now
        );

        return newData;
    }

    private GamePlayData LoadGame()
    {
        GamePlayData newGame = new GamePlayData(
            currentPlayer: (CellColor)playerPrefs.GetInt(GameDataKeys.CurrentPlayer, (int)CellColor.Empty),
            nextPlayer: (CellColor)playerPrefs.GetInt(GameDataKeys.NextPlayer, (int)CellColor.Empty),
            currentScore: playerPrefs.GetInt(GameDataKeys.CurrentScore, 0),
            board: disc.Load<BoardData>(GameDataFiles.BoardData)
            );

        return newGame;
    }
}
