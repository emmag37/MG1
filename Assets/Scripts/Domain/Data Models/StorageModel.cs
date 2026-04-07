using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ScoreHistory
{
    // properties
    private const int Max = 10;
    public IReadOnlyList<int> ROScores => scores;

    // serialized fields
    private List<int> scores = new List<int>();

    // functions
    public bool AddScore(int score)
    {
        if (scores.Count < Max)
        {
            InsertScore(score);

            return true;
        }
        else if (score > scores[Max - 1])
        {
            // remove the bottom score
            scores.RemoveAt(Max - 1);

            InsertScore(score);
        }

        return false;
    }

    // uses a modified binary search algorithm to maintain sorted property, O(logn)
    private void InsertScore(int score)
    {
        int index = 0;  // default for empty list

        int a = 0;
        int b = scores.Count - 1;
        while (a <= b)
        {
            int m = (b - a) / 2;

            // check if found
                // need to add edge cases
            if (score == m || (score < m && (m == 0 || score > m - 1))) // m - 1 must exist
            {
                index = m;
                break;
            }
            else if (score > m && (m == scores.Count - 1 || score < m + 1)) // m + 1 must exist
            {
                index = m + 1;
                break;
            }

            // update a and b
            if (score < m)
            {
                b = m - 1;
            }
            else
            {
                a = m + 1;
            }
        }

        scores.Insert(index, score);
    }
}


