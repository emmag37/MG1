using UnityEngine;
using UnityEngine.UI;
using System;


public class ScoreScrollItem : MonoBehaviour, IScrollItem<int>
{
    // ==================================================
    // Public Fields
    // ==================================================
    public GameObject GameObject => gameObject;

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Text rankText;
    [SerializeField] private Text scoreText;


    // ==================================================
    // Public Methods
    // ==================================================

    public void Set(int data, int index)
    {
        if (index + 1 > UIConstants.NumScores)
            throw new ArgumentOutOfRangeException($"[ScoreScrollItem] Index {index} out of range");

        scoreText.text = $"{data}";
        rankText.text = $"{index + 1}";
    }
}
