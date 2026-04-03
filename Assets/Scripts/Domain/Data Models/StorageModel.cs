using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ScoreHistory
{
    public List<int> Scores = new List<int>();

    public IReadOnlyList<int> ROScores => Scores;
}
