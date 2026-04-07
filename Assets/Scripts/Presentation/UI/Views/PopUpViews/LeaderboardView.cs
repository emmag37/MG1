using UnityEngine;

public class LeaderboardView : PopUpView<IRuntimeData>
{
    // ==================================================
    // Private Fields
    // ==================================================
    //private VerticalScrollList listView;


    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(IRuntimeData data)
    {
        //if (listView == null) listView = GetComponent<VerticalScrollList>();

        // populate the list view

        base.Show(data);
    }

}
