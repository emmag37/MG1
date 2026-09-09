using UnityEngine;
using UnityEngine.UI;


public class ScoreScrollItem : MonoBehaviour, IScrollItem<int>
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text scoreText;

    public GameObject GameObject => gameObject;

    public void Set(int data, int index)
    {
        scoreText.text = $"{data}";
        rankText.text = $"{index + 1}";
    }
}
