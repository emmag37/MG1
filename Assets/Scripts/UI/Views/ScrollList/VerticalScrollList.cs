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
