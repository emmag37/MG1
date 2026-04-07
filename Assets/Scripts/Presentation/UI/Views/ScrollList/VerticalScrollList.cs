using UnityEngine;
using System.Collections.Generic;

// type this to the data type of list
// type the list item classes to the data type of list as well?
public class VerticalScrollList<T> : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject listItem;   // prefab
    
    public void Populate(IReadOnlyList<T> list)
    {
        // clear existing list
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (T data in list)
        {
            // create the list item
            ItemView<T> item = Instantiate(listItem, content).GetComponent<ItemView<T>>();
            item.Set(data);
        }
    }
}
