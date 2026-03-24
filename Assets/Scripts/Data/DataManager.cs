using UnityEngine;

public class DataManager : MonoBehaviour
{
    // ==================================================
    // Public Properties
    // ==================================================

    public static DataManager Instance { get; private set; }

    public PlayerProfile Profile { get; private set; }
    public GameSettings Settings { get; private set; }

    // ================================
    // Private Fields
    // ================================

    private static DataManager instance;


    // ================================
    // Unity Lifecycle
    // ================================

    void Awake()
    {
        Instance = this;
        Load();
    }


    // ================================
    // Profile Methods
    // ================================

    public void SetScore(int score)
    {
        if (score > Profile.HighScore)
        {
            Profile.HighScore = score;
        }

        Profile.RecentScore = score;
    }

    public void SetUsername(string name)
    {
        Profile.Username = name;
    }


    // ================================
    // Settings Methods
    // ================================

    public void SetLaunched()
    {
        Settings.HasLaunched = 1;
    }

    public void SetMusicOn(int on)
    {
        Settings.MusicOn = on;
    }

    public void SetEffectsOn(int on)
    {
        Settings.EffectsOn = on;
    }


    // ================================
    // Private Methods
    // ================================

    private void Load()
    {
        Profile = new PlayerProfile();
        Profile.Load();

        Settings = new GameSettings();
        Settings.Load();
    }

}
