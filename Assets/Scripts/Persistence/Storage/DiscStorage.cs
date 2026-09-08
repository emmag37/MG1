using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;

// todo:
    // separate out reusable pieces for your library
    // finish BackupCorruptData
    // error check save

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
    private string GetPath(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("[JsonFileStorage] File name is null or empty", nameof(fileName));

        return Path.Combine(Application.persistentDataPath, fileName);
    }

    // check
    // requires non-null data and file name
    // doesn't check any of its exceptions
    public void Save<T>(string fileName, T data)
    {
        Debug.Assert(data != null, "[JsonFileStorage] Attempted save with null data");

        string path = GetPath(fileName);
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
    }

    // done
    // requires generic exception catch for all other exceptions
    // note : file save exception is not built in
    public async Task SaveWithRetries<T>(string fileName, T data, int retries = 3)
    {
        for (int i = 1; i <= retries; i++)
        {
            try
            {
                Save<T>(fileName, data);
                return;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogError($"[JsonFileStorage] Save {fileName} failed attempt {i} with {e}");
                if (i < retries)
                    await Task.Delay(100);
                else
                    throw e;
            }
        }
    }

    // done
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

    // done
    // throws FileLoadException
    public async Task<T> LoadWithRetries<T>(string fileName, int retries = 3) where T : new()
    {
        for (int i = 1; i <= retries; i++)
        {
            try
            {
                return Load<T>(fileName);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogError($"[JsonFileStorage] Load {fileName} failed attempt {i} with {e}");
                if (i < retries)
                    await Task.Delay(100);
                else
                    throw e;
            }
        }
        throw new FileLoadException(); // unreachable, for compiler
    }

    // done
    // keep track of the number of backups?
    public void BackupCorruptData(string path)
    {
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
