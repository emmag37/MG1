using UnityEngine;

public static class Logger
{
    public static void Error(string message) => Debug.LogError(message);
    public static void Warning(string message) => Debug.LogWarning(message);
}
