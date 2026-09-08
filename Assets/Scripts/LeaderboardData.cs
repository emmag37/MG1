using UnityEngine;
using System;

[Serializable]
public class LeaderboardData : IComparable<LeaderboardData>   // needs to inherit comparable, also needs to be serializable?
{
    // data items to hold
        // note - this data set does not score rank
    public int Avatar;
    public string Username;
    public int Score;

    // comparable
    public int CompareTo(LeaderboardData other)
    {
        if (other == null) return 1; // error

        return Score - other.Score;
    }
}
