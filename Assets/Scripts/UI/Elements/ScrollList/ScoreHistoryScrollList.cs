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


public class UIAlternatingVerticalScrollList<T> : UIVerticalScrollList<T>
{
    private GameObject itemView2;

    public UIAlternatingVerticalScrollList(Transform content, GameObject itemView1, GameObject itemView2)
        : base(content, itemView1)
    {
        this.itemView2 = itemView2;
    }

    protected override void CreateItem(T data, int index)
    {
        IScrollItem<T> scrollItem;
        if (index % 2 == 0)
            scrollItem = Object.Instantiate(itemView, content).GetComponent<IScrollItem<T>>();
        else
            scrollItem = Object.Instantiate(itemView2, content).GetComponent<IScrollItem<T>>();

        scrollItem.Set(data, index);
    }
}
