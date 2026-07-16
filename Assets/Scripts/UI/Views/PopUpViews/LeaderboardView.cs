using UnityEngine;

// currently not using this screen
public class LeaderboardView : PopUpView
{
    // ==================================================
    // Private Fields
    // ==================================================
    private LeaderboardScrollList listView;


    // ==================================================
    // Base Class Methods
    // ==================================================

    protected override void SetInfo(IRuntimeData data)
    {
        if (data is not IGameData gameData)
        {
            Debug.Log($"data passed to leaderboard view is not game data, type: {data?.GetType().Name}");
            return;
        }

        if (listView == null) listView = GetComponent<LeaderboardScrollList>();

        listView.Populate(gameData.LeaderboardRanking);
    }
}
