using UnityEngine;
using UnityEngine.UI;

public class ScoreItemView : ItemView<int>
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text scoreText;

    public override void Set(int data, int index)
    {
        scoreText.text = $"{data}";
        rankText.text = $"{index + 1}";
    }
}
