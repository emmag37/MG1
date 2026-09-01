using UnityEngine;
using System.IO;
using System;

// separate out the reusable pieces to put into library

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

public class JsonFileStorage : IStorage
{
    private string GetPath(string fileName) // what does this function do on faluire?
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("[JsonFileStorage] File name is null or empty", nameof(fileName));

        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save<T>(string fileName, T data)
    {
        string path = GetPath(fileName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    // exceptions thrown (recoverable): file not found, io, and unauthorized access
    public T Load<T>(string fileName) where T : new()
    {
        string path = GetPath(fileName);    // safe

        if (!File.Exists(path)) // safe
            return new T();

        string json = File.ReadAllText(path);   // catch exceptions in bootstrap - retry loading

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning($"[JsonFileStorage] Null or empty read from file {fileName}");
            return new T();
        }

        try
        {
            T result = JsonUtility.FromJson<T>(json);
            if (result == null)
            {
                Debug.LogError("[JsonFileStorage] From Json returned null result");
                BackupCorruptData(path);

                return new T();
            }

            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonFileStorage] From Json failed with exception {e}");
            BackupCorruptData(path);

            return new T();
        }
    }

    private void BackupCorruptData(string path)
    {
        // show error message

        try
        {
            string backupPath = $"{path}.corrupt_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";
            File.Move(path, backupPath);
            Debug.LogWarning($"[JsonFileStorage] Moved corrupt data file from {path} to {backupPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonFileStorage] Failed to backup data at {path} with exception {e}");
        }
    }
}
