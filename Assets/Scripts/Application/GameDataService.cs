using UnityEngine;
using System;

public class GameDataService
{
    // ==================================================
    // Public Fields
    // ==================================================
    public IGameData GetGameData() => data;

    // ==================================================
    // Private Fields
    // ==================================================
    private PlayerPrefsStorage storage;
    private GameData data;


    // ==================================================
    // Constructor/Initializer
    // ==================================================

    public GameDataService(PlayerPrefsStorage storage)
    {
        this.storage = storage;

        data = Load();
    }

    // ==================================================
    // Public Methods
    // ==================================================

    // called on game over only
    public void SetScore(int score)
    {
        data.Score = score;

        // run data save protocol for score history
    }

    // also save this in score history, don't want users to be able to change this easily
    public void SetHighScore(int highScore)
    {
        data.HighScore = highScore;
        storage.SetInt(GameDataKeys.HighScore, highScore);
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private GameData Load()
    {
        GameData newData = new GameData(
            score: 0,
            highScore: storage.GetInt(GameDataKeys.HighScore, 0)
        );

        return newData;
    }
}
