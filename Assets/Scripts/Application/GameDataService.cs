using UnityEngine;
using System;

public class GameDataService
{
    // ==================================================
    // Public Fields
    // ==================================================
    public IGameData GetGameData() => data;

    // ==================================================
    // Events
    // ==================================================
    public event Action<int, int> NewScore;

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

    public void SetScore(int score)
    {
        data.Score = score;

        if (score > data.HighScore)
        {
            data.HighScore = score;
            storage.SetInt(GameDataKeys.HighScore, score);
        }

        NewScore?.Invoke(data.Score, data.HighScore);
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
