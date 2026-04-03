using UnityEngine;
using UnityEngine.UI;

public class ScoreHistoryView : PopUpView<IGameData>
{
    public override void Show(IGameData data)
    {
        foreach (int score in data.ScoreHistory)
        {
            Debug.Log($"score: {score}");
        }

        base.Show(data);
    }
}
