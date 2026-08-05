using UnityEngine;
using System.Collections.Generic;

// type this to the data type of list
// type the list item classes to the data type of list as well?
public abstract class VerticalScrollList<T> : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] protected GameObject listItem;   // prefab

    public void Populate(IReadOnlyList<T> list)
    {
        Clear();

        for (int i = 0; i < list.Count; i++)
        {
            CreateItem(list[i], i);
        }
    }

    private void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    protected ItemView<T> InstantiateItem(GameObject template, T data, int index)
    {
        ItemView<T> item = Instantiate(template, content).GetComponent<ItemView<T>>();
        item.Set(data, index);

        return item;
    }

    protected virtual ItemView<T> CreateItem(T data, int index)
    {
        ItemView<T> item = InstantiateItem(listItem, data, index);

        return item;
    }
}


// potentially give a size to pre-populate
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

public interface IScrollItem<T>
{
    void Set(T data, int index);
}
