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
    public SettingsData Settings;
    public ProfileData Profile;
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
    public ValidatedUsername Username = new ValidatedUsername();
    public CellColor Avatar = CellColor.Color1;
    public ScoreHistory ScoreList = new ScoreHistory();
}
 

[Serializable]
public class ScoreHistory : CappedRankedList<int>
{
    // only hold top 10 scores in score history
    public ScoreHistory() : base(10) { }

    public int HighScore()
    {
        if (list.Count > 0)
            return list[0];
        else
            return 0;
    }
}

public sealed record FinalScoreData(int score, int highScore) : IUIData; // this is what is passed to the game over screen

/* to be implemented in the future
 * 
[Serializable]
public class LeaderboardRanking : CappedRankedList<LeaderboardData>
{
    // hold the top 50 scores
    public LeaderboardRanking() : base(50) { }
}
*/
