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
    protected int maxItems;

    protected IScrollItem<T>[] scrollItems;
    
    public UIVerticalScrollList(Transform content, GameObject itemView, int maxItems)
    {
        this.content = content;
        this.itemView = itemView;
        this.maxItems = maxItems;

        scrollItems = new IScrollItem<T>[maxItems];
    }

    // 2-phase construction
    public virtual void Populate()
    {
        for (int i = 0; i < maxItems; i++)
        {
            scrollItems[i] = Object.Instantiate(itemView, content).GetComponent<IScrollItem<T>>();
            scrollItems[i].GameObject.SetActive(false);
        }
    }

    public void SetList(IReadOnlyList<T> dataList)
    {
        // should you actually clear these? is clear necessary? 

        for (int i = 0; i < dataList.Count; i++)
        {
            scrollItems[i].GameObject.SetActive(true);
            scrollItems[i].Set(dataList[i], i);
        }
    }
}


