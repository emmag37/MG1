using UnityEngine;

// alt : alternating item view prefabs
/*
public class UIAltVertScrollList<T> : UIVerticalScrollList<T>
{
    private GameObject itemView2;

    public UIAltVertScrollList(Transform content, GameObject itemView1, GameObject itemView2, int maxItems)
        : base(content, itemView1, maxItems)
    {
        this.itemView2 = itemView2;
    }

    public override void Populate()
    {
        for (int i = 0; i < maxItems; i++)
        {
            if (i % 2 == 0)
                scrollItems[i] = Object.Instantiate(itemView, content).GetComponent<IScrollItem<T>>();
            else
                scrollItems[i] = Object.Instantiate(itemView2, content).GetComponent<IScrollItem<T>>();

            scrollItems[i].GameObject.SetActive(false);
        }
    }
}
*/