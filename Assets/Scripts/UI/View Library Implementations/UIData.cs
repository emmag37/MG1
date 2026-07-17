using UnityEngine;
using System;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

[Serializable]
public class UIData : IUIData
{
    // this is the data that actually gets stored to disc
    public SettingsData SettingsData;
    public ProfileData ProfileData;
}

[Serializable]
public class SettingsData : IUIData
{
    public bool MusicOn;
    public bool SFXOn;
    public bool VibrateOn;
}

[Serializable]
public sealed class ProfileData : IUIData
{
    public string Username;
    public CellColor Avatar;
    public ScoreHistory ScoreList;
}


[Serializable]
public class ScoreHistory : CappedRankedList<int>
{
    // only hold top 10 scores in score history
    public ScoreHistory() : base(10) { }
}

public sealed record ScoreData(int score, int highScore) : IUIData; // this is what is passed to the game over screen
