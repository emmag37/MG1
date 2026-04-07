using UnityEngine;
using UnityEngine.UI;

public class ScoreItemView : ItemView<int>
{
    [SerializeField] private Text scoreText;

    public override void Set(int data)
    {
        scoreText.text = $"{data}";
    }
}
