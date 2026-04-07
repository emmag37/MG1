using UnityEngine;
using System;
using System.Collections.Generic;

public class GameDataService
{
    // ==================================================
    // Public Fields
    // ==================================================
    public IGameData GetGameData() => data;

    // ==================================================
    // Private Fields
    // ==================================================
    private DiscStorage disc;
    private PlayerPrefsStorage playerPrefs;

    private GameData data;


    // ==================================================
    // Constructor/Initializer
    // ==================================================

    public GameDataService(DiscStorage disc, PlayerPrefsStorage playerPrefs)
    {
        this.disc = disc;
        this.playerPrefs = playerPrefs;

        data = Load();
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void SetHighScore(int highScore)
    {
        data.HighScore = highScore;
        playerPrefs.SetInt(GameDataKeys.HighScore, highScore);
    }

    public void AddScore(int score)
    {
        data.Score = score;

        ScoreHistory history = disc.Load<ScoreHistory>(GameDataFiles.ScoreHistory);
        history.AddScore(score);

        disc.Save<ScoreHistory>(GameDataFiles.ScoreHistory, history);
        data.ScoreHistory = history.ROScores;
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private GameData Load()
    {
        GameData newData = new GameData(
            score: 0,
            highScore: playerPrefs.GetInt(GameDataKeys.HighScore, 0),
            scoreHistory: disc.Load<ScoreHistory>(GameDataFiles.ScoreHistory).ROScores
        );

        return newData;
    }
}
