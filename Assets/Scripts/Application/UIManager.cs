using UnityEngine;
using System;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
    // ==================================================
    // Events
    // ==================================================

    public event Action ShowTutorial;
    public event Action<PlayerProfile> ShowHome;

    // ==================================================
    // Inspector Fields
    // ==================================================

    [SerializeField] private GameManager gameManager;

    // ==================================================
    // Private Fields
    // ==================================================

    private DataManager data => DataManager.Instance;
    

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Start()
    {
        ShowHome?.Invoke(data.Profile);

        if (data.Settings.HasLaunched == 0)
        {
            ShowTutorial?.Invoke();
            data.SetLaunched();
        }
    }


    // ==================================================
    // Public Methods
    // ==================================================

    public void UpdateUsername(string name)
    {
        data.SetUsername(name);
    }

    public void UpdateAvatar(CellColor color)
    {
        data.SetAvatar((int)color);
        // refresh views
    }

    public void UpdateMusicOn(int on)
    {
        data.SetMusicOn(on);
        EventBus.Publish(new UpdateSettingsEvent());
    }

    public void UpdateEffectsOn(int on)
    {
        data.SetEffectsOn(on);
        EventBus.Publish(new UpdateSettingsEvent());
    }


    // ==================================================
    // View Controller Methods
    // ==================================================

    public void HandleStartPressed()
    {
        gameManager.StartGame();
    }

    public void HandleExitGamePressed()
    {
        gameManager.ExitGame();
    }

    public void HandlePausePressed()
    {
        gameManager.PauseGame();
    }

    public void HandleResumePressed()
    {
        gameManager.ResumeGame();
    }
}
