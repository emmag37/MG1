using UnityEngine;

// uses two list items to alternate colors
public class ScoreHistoryScrollList : VerticalScrollList<int>
{
    [SerializeField] private GameObject alternateListItem;

    protected override ItemView<int> CreateItem(int data, int index)
    {
        if (index % 2 == 0)
        {
            return InstantiateItem(listItem, data, index);
        }
        else
        {
            return InstantiateItem(alternateListItem, data, index);
        }
    }

 }
