using UnityEngine;

[RequireComponent(typeof(VerticalScrollList))]
public class ScoreHistoryView : PopUpView<IGameData>
{
    // ==================================================
    // Private Fields
    // ==================================================
    private VerticalScrollList listView;


    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(IGameData data)
    {
        if (listView == null) listView = GetComponent<VerticalScrollList>();

        listView.Populate(data.ScoreHistory);

        base.Show(data);
    }

}
