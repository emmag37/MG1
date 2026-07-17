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
    public SettingsData settingsData;
    public ProfileData profileData;
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
    public List<int> ScoreHistory;
}

public sealed record ScoreData(int score, int highScore) : IUIData; // this is what is passed to the game over screen
