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
    // settings
    public AudioSettings AudioSettings;
    public bool VibrationOn = true;                

    // profile
    public ProfileData Profile = new();
}

[Serializable]
public class AudioSettings
{
    public bool MusicOn = true;
    public bool SFXOn = true;
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

// data patches

public sealed record AvatarPatch(CellColor avatar) : IUIPatch;
public sealed record UsernamePatch(string username) : IUIPatch;
