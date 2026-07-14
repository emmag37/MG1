using UnityEngine;
using System;

/// <summary>
/// Controls the game states and initiates plays.
/// Bridges communication between player instances, the board, and UI updates.
/// </summary>
public class GameManager
{
    // ================================
    // Private Fields
    // ================================
    private GameDataService dataService;
    private Board board;

    // add back in tutorial

    private bool inProgress = false;


    // ================================
    // Constructor
    // ================================

    // turn this into a constructor
    public GameManager(GameDataService dataService, Board board)
    {
        this.dataService = dataService;
        this.board = board;

        board.Initialize(dataService);

        if (dataService.GetGameData().InProgress == true)   // to subscribe to events
            SetInProgress(true);
    }


    // ================================
    // UI Commands
    // ================================

    // should tutorial be in first or second branch?
    // results in active game state with play enabled
    public void Play()
    {
        // two options: game in progress, or not
        // if game in progress, you actually don't need to do anything

        if (inProgress) return;

        // create a new game
        board.FreshGame();
        EventBus.Publish(new StartGameEvent());

        SetInProgress(true);
    }

    // pause == true to pause, pause == false to unpause
    public void Pause(bool pause)
    {
        board.PauseGame(pause);
    }

    public void Restart()
    {
        SetInProgress(false);     // trigger a fresh game

        Play();
    }

    public void StartTutorial()
    {
        // start the tutorial
    }

    public void SkipTutorial()
    {
        // skip to the end of the tutorial
    }


    // ================================
    // Event Handlers
    // ================================

    private void HandleFullBoard()
    {
        SetInProgress(false);

        EventBus.Publish(new GameOverEvent { Data = dataService.GetGameData() });
    }


    // ================================
    // Private Methods
    // ================================

    // always subscribes/unsubscribes when this state changes
    private void SetInProgress(bool inProgress)
    {
        Debug.Log($"set in progress: {inProgress}");

        this.inProgress = inProgress;
        dataService.SetInProgress(inProgress);

        if (inProgress)
            board.FullBoard += HandleFullBoard;
        else
            board.FullBoard -= HandleFullBoard;
    }

}
