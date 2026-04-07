using UnityEngine;

public class LeaderboardScrollList : VerticalScrollList<LeaderboardData>
{
    // set the rank according to the list index
    protected override ItemView<LeaderboardData> CreateItem(LeaderboardData data, int index)
    {
        LeaderboardItemView item = base.CreateItem(data, index) as LeaderboardItemView;

        item.SetRank(index + 1);    // need to start at 1, not 0
        return item;
    }
}
