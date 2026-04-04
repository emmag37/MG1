using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreHistoryView : PopUpView<IGameData>
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject listItem;   // prefab
    

    public override void Show(IGameData data)
    {
        PopulateList(data.ScoreHistory);

        base.Show(data);
    }

    private void PopulateList(IReadOnlyList<int> scoreHistory)
    {
        // clear existing list
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (int score in scoreHistory)
        {
            Debug.Log($"score: {score}");

            // create the list item
            GameObject item = Instantiate(listItem, content);

            // set its text to score
            Text text = item.GetComponent<Text>();
            text.text = $"{score}";
        }
    }
}
