using UnityEngine;
using System;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }        // for records
}


[Serializable]
public class UIData : IUIData
{
    // init flags
    public bool HasLaunched = false;
    public bool InProgress = false;

    // profile
    public ProfileData Profile = new();

    // settings
    public AudioSettings AudioSettings = new();
    public bool VibrationOn = true;
}

[Serializable]
public class ProfileData : IUIData
{
    public string Username = "default-username";
    public CellColor Avatar = CellColor.Color1;

    public ScoreHistory ScoreList = new();
}

[Serializable]
public class ScoreHistory : CappedRankedList<int>
{
    public ScoreHistory() : base(UIConstants.NumScores) { }

    public int HighScore()
    {
        if (list.Count > 0)
            return list[0];
        else
            return 0;
    }
}

public sealed record FinalScoreData(int score, int highScore) : IUIData;

// data patches
public sealed record AvatarPatch(CellColor avatar) : IUIPatch;
public sealed record UsernamePatch(string username) : IUIPatch;
