using UnityEngine;
using System;

/// <summary>
/// Controls the game states and initiates plays.
/// Bridges communication between player instances, the board, and UI updates.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private Board board;
    [SerializeField] private TutorialController tutorial;

    // ================================
    // Private Fields
    // ================================
    private GameDataService dataService;

    private GameState state;
    private bool gameLoaded;
    private bool playEnabled;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnDestroy()
    {
        board.FullBoard -= HandleFullBoard;
        tutorial.TutorialComplete -= HandleTutorialComplete;
    }


    // ================================
    // Initialize
    // ================================

    public void Initialize(GameDataService dataService)
    {
        this.dataService = dataService;

        board.Initialize(dataService);

        board.FullBoard += HandleFullBoard;
        tutorial.TutorialComplete += HandleTutorialComplete;

        state = dataService.GetGameData().State;

        // initialize values
        playEnabled = true;
        gameLoaded = false;
    }


    // ================================
    // UI Commands
    // ================================

    // should tutorial be in first or second branch?
    // results in active game state with play enabled
    public void Play()
    {
        Debug.Log($"state: {state}, enabled: {playEnabled}");
        if (state == GameState.Inactive && !playEnabled)                                        // restart
        {
            playEnabled = true;

            Reset();
            NewGame();
        }
        else if (state == GameState.Inactive && playEnabled)   // fresh from home, game over
        {
            Debug.Log("fresh game branch");
            NewGame(); // can assume all game elements are reset
        }
        else if (state == GameState.Tutorial && playEnabled)
        {
            RunTutorial();
        }
        else if (state == GameState.Active && !playEnabled)
        {
            playEnabled = true;
            board.PauseGame(false);                                            // resume gameplay
        }
        else if (state == GameState.Active && playEnabled && !gameLoaded)
        {
            Debug.Log("load game - turned off");

            //LoadGame();
            board.StartGame();
            EventBus.Publish(new StartGameEvent());  // continue the gameplay

            gameLoaded = true;
        }
        else
        {
            Debug.Log("continue game");
            playEnabled = true;
        }
    }

    public void Pause()
    {
        Debug.Assert(state == GameState.Active, $"Pause called with non-active state, state = {state}");

        playEnabled = false;
        board.PauseGame(true);
    }

    public void Restart()
    {
        // only need to change states
        state = GameState.Inactive;
        dataService.SetState(GameState.Inactive);

        Play();
    }

    public void SkipTutorial()
    {
        Debug.Assert(state == GameState.Tutorial, $"Skip tutorial called with state: {state}");

        tutorial.CompleteTutorial();
    }


    // ================================
    // Event Handlers
    // ================================

    private void HandleFullBoard()
    {
        Debug.Assert(state == GameState.Active && playEnabled, $"Initiate game over from invalid state: {state}, {playEnabled}");

        state = GameState.Inactive;
        dataService.SetState(GameState.Inactive);

        EventBus.Publish(new GameOverEvent { Data = dataService.GetGameData() });
        Reset();
    }

    // tutorial finsihed event
    private void HandleTutorialComplete()
    {
        Debug.Assert(state == GameState.Tutorial, $"Tutorial complete but state = {state}");

        state = GameState.Inactive;
        dataService.SetState(state);
        playEnabled = true;
    }

    // ================================
    // Private Methods
    // ================================

    // game state helpers
    private void LoadGame()
    {
        IGamePlayData game = dataService.GetGamePlayData();

        // load the board - must always load the board before players
        foreach (CellEntry cell in game.Board.Cells)
        {
            Debug.Log($"add item: ({cell.x}, {cell.y}), {cell.color}");
            //board.AddNonPlayer(new Vector2Int(cell.x, cell.y), (CellColor)cell.color);
        }

        // this is not working properly
        //EventBus.Publish(new SpawnPlayerEvent { Color = game.CurrentPlayer, NextColor = game.NextPlayer });
    }

    private void Reset()
    {
        Debug.Assert(state == GameState.Inactive, $"Reset called from state: {state}");
    }

    private void NewGame()
    {
        Debug.Assert(state == GameState.Inactive, $"New game called from state: {state}");

        Debug.Log("new game!!!");

        state = GameState.Active;
        dataService.SetState(state);

        board.StartGame();
        EventBus.Publish(new StartGameEvent());   // prepare systems not owned by the game manager
    }

    public void RunTutorial()
    {
        Debug.Assert(state == GameState.Tutorial, $"Tutorial run from state: {state}");

        //tutorial.Initialize(board);
        tutorial.StartTutorial();
    }

}
