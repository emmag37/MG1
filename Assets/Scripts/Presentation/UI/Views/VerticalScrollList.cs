using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VerticalScrollList : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject listItem;   // prefab
    
    public void Populate(IReadOnlyList<int> list)
    {
        // clear existing list
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (int data in list)
        {
            Debug.Log($"add item: {data}");

            // create the list item
            GameObject item = Instantiate(listItem, content);

            // set the text 
            Text text = item.GetComponent<Text>();
            text.text = $"{data}";
        }
    }
}
