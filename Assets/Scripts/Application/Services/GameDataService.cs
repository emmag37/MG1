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

    public void SetState(GameState state)
    {
        data.State = state;
        playerPrefs.SetInt(GameDataKeys.State, (int)state);
    }

    public void AddScore(int score)
    {
        data.Score = score;
        if (score > data.HighScore)
        {
            data.HighScore = score;
            playerPrefs.SetInt(GameDataKeys.HighScore, score);
        }

        ScoreHistory history = disc.Load<ScoreHistory>(GameDataFiles.ScoreHistory);
        bool added = history.TryAddValue(score);
        disc.Save(GameDataFiles.ScoreHistory, history);

        if (added)
        {
            data.ScoreHistory = history.ROList;
        }
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private GameData Load()
    {
        GameData newData = new GameData(
            state: (GameState)playerPrefs.GetInt(GameDataKeys.State, (int)GameState.Tutorial),  // always run tutorial for the first use
            score: 0,
            highScore: playerPrefs.GetInt(GameDataKeys.HighScore, 0),
            scoreHistory: disc.Load<ScoreHistory>(GameDataFiles.ScoreHistory).ROList,
            leaderboardRanking: (new LeaderboardRanking()).ROList    // need to load from the cloud, empty list for now
        );

        return newData;
    }
}
