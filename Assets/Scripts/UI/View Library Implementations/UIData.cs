using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public sealed record ProfileData(bool musicOn, bool sfxOn, string Username, CellColor Avatar) : IUIData;

public sealed record ScoreData(int score, int highScore) : IUIData;
