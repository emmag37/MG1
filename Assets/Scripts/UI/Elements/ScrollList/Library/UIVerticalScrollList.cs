using UnityEngine;
using System.Collections.Generic;

// maybe give it a function to update so that it doesn't have to be populated every time
// like it recieves event for a new score, then updates in the background
    // would have to change the maintain order property to here
    // then you would have to redo the text for the alternating list anyway

public class UIVerticalScrollList<T>
{
    protected Transform content;
    protected GameObject itemView;    // prefab - must have component that extends IScrollItem
    
    public UIVerticalScrollList(Transform content, GameObject itemView)
    {
        this.content = content;
        this.itemView = itemView;
    }

    public void Populate(IReadOnlyList<T> dataList)
    {
        Clear();

        for (int i = 0; i < dataList.Count; i++)
        {
            CreateItem(dataList[i], i);
        }
    }

    protected virtual void CreateItem(T data, int index)
    {
        IScrollItem<T> scrollItem = Object.Instantiate(itemView, content).GetComponent<IScrollItem<T>>();
        scrollItem.Set(data, index);
    }

    private void Clear()
    {
        foreach (Transform child in content)
        {
            Object.Destroy(child.gameObject);
        }
    }
}


