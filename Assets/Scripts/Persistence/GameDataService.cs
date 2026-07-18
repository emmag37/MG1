using UnityEngine;
using System;
using System.Collections.Generic;

public class GameDataService : MonoBehaviour
{
    // ==================================================
    // Public Fields
    // ==================================================
    public bool InProgress;

    // ==================================================
    // Private Fields
    // ==================================================
    private DiscStorage disc;
    private PlayerPrefsStorage playerPrefs;     // necessary to save in progress

    private GameData game;

    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize(DiscStorage disc, PlayerPrefsStorage playerPrefs)
    {
        this.disc = disc;
        this.playerPrefs = playerPrefs;

        LoadData();
    }


    // ==================================================
    // Load/Save
    // ==================================================

    private void LoadData()
    {
        InProgress = playerPrefs.GetBool(InitKeys.InProgress, false);

        if (InProgress)
            game = disc.Load<GameData>(DataFiles.GameData);
        else
            game = new GameData();
    }

    private void SaveData()
    {
        playerPrefs.SetBool(InitKeys.InProgress, InProgress);

        if (InProgress)
            disc.Save<GameData>(DataFiles.GameData, game);
        
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // app is being backgrounded
        if (pauseStatus) SaveData();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // app lost focus (backgrounded on some platforms, alt-tabbed on desktop)
        if (!hasFocus) SaveData();
    }
}
