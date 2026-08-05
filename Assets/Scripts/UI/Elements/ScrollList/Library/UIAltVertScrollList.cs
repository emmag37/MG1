using UnityEngine;


public class UIAltVertScrollList<T> : UIVerticalScrollList<T>
{
    private GameObject itemView2;

    public UIAltVertScrollList(Transform content, GameObject itemView1, GameObject itemView2)
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
