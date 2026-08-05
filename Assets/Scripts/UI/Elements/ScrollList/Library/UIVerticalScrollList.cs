using UnityEngine;
using System.Collections.Generic;


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


