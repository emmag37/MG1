using UnityEngine;

public class LeaderboardView : PopUpView<IGameData>
{
    // ==================================================
    // Private Fields
    // ==================================================
    private LeaderboardScrollList listView;


    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(IGameData data)
    {
        if (listView == null) listView = GetComponent<LeaderboardScrollList>();

        listView.Populate(data.LeaderboardRanking);

        base.Show(data);
    }

}
