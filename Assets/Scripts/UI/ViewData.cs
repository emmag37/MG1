
public class ViewData
{
    public int GameScore { get; private set; }
    public int HighScore { get; private set; }


    public ViewData(int score, int highScore)
    {
        GameScore = score;
        HighScore = highScore;
    }
}
