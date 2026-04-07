using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItemView : ItemView<LeaderboardData>
{
    // inspector fields
    [SerializeField] private Text rankText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Text usernameText;
    [SerializeField] private Text scoreText;

    // functions
    public override void Set(LeaderboardData data)
    {
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);
        usernameText.text = data.Username;
        scoreText.text = $"{data.Score}";
    }

    public void SetRank(int rank)
    {
        rankText.text = $"{rank}";
    }
}
