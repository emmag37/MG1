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
    public event Action<int> NewHighScore;

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

    public void SetHighScore(int highScore)
    {
        data.HighScore = highScore;
        storage.SetInt(GameDataKeys.HighScore, highScore);

        NewHighScore?.Invoke(highScore);
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
