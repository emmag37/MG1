using UnityEngine;
using System.IO;

public static class DataFiles
{
    public const string UIData = "uIData.json";
    public const string GameData = "gameData.json";
}

public interface IStorage
{
    void Save<T>(string fileName, T data);
    T Load<T>(string fileName) where T : new();
}

public class DiscStorage : IStorage
{
    private string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save<T>(string fileName, T data)
    {
        string path = GetPath(fileName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public T Load<T>(string fileName) where T : new()
    {
        string path = GetPath(fileName);

        if (!File.Exists(path))
            return new T();

        string json = File.ReadAllText(path);

        if (string.IsNullOrEmpty(json))
            return new T();

        return JsonUtility.FromJson<T>(json);
    }
}
