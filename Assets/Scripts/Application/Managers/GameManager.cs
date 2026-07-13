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
    [SerializeField] private Board boardView;
    [SerializeField] private TutorialController tutorial;

    // ================================
    // Private Fields
    // ================================
    private GameDataService dataService;

    private GameState state;
    private bool gameLoaded;
    private bool playEnabled;

    private int score;
    private int highScore;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnDestroy()
    {
        boardView.TurnCompleted -= HandleTurnCompleted;
        tutorial.TutorialComplete -= HandleTutorialComplete;
    }


    // ================================
    // Initialize
    // ================================

    public void Initialize(GameDataService dataService)
    {
        this.dataService = dataService;

        boardView.TurnCompleted += HandleTurnCompleted;
        tutorial.TutorialComplete += HandleTutorialComplete;

        highScore = dataService.GetGameData().HighScore;
        state = dataService.GetGameData().State;


        // initialize values
        playEnabled = true;
        gameLoaded = false;

        score = 0;
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
            EventBus.Publish(new ResumeGameEvent());                                            // resume gameplay
        }
        else if (state == GameState.Active && playEnabled && !gameLoaded)
        {
            Debug.Log("load game - turned off");

            //LoadGame();
            EventBus.Publish(new StartGameEvent { Data = dataService.GetGameData() });  // continue the gameplay

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
        EventBus.Publish(new PauseGameEvent());
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

    // moves the player back to start or on the board.
    // if on the board, executes the player's turn.
    private void HandleTurnCompleted(bool fullBoard, int points, (int, int) index)  // turn this into a local event
    {
        if (state == GameState.Tutorial) return;

        Debug.Assert(state == GameState.Active, $"Turn ran during invalid state: {state}");
        //Debug.Assert(activePlayer, "Player turn completed but no active player");

        if (fullBoard)
        {
            InitiateGameOver();
            return;
        }

        if (points > 0)
        {
            score += points;
            dataService.UpdateScore(score);
            EventBus.Publish(new ScoreUpdateEvent { Score = score, HighScore = dataService.GetGameData().HighScore });
        }

        dataService.SaveTurn(score, index);

        //if (state == GameState.Active) SpawnNewPlayer();
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

        // load the current score and players
        score = game.CurrentScore;
        dataService.UpdateScore(score);

        // this is not working properly
        //EventBus.Publish(new SpawnPlayerEvent { Color = game.CurrentPlayer, NextColor = game.NextPlayer });
    }

    private void Reset()
    {
        Debug.Assert(state == GameState.Inactive, $"Reset called from state: {state}");

        boardView.Reset();

        score = 0;
        dataService.UpdateScore(score);
        highScore = dataService.GetGameData().HighScore;


        dataService.ResetGame();
    }

    private void NewGame()
    {
        Debug.Assert(state == GameState.Inactive, $"New game called from state: {state}");

        Debug.Log("new game!!!");

        state = GameState.Active;
        dataService.SetState(state);

        EventBus.Publish(new StartGameEvent { Data = dataService.GetGameData() });   // prepare systems not owned by the game manager
        //SpawnNewPlayer();
    }

    public void RunTutorial()
    {
        Debug.Assert(state == GameState.Tutorial, $"Tutorial run from state: {state}");

        //tutorial.Initialize(board);
        tutorial.StartTutorial();
    }

    private void InitiateGameOver()    // called on a game over  - turn this into a local event
    {
        Debug.Assert(state == GameState.Active && playEnabled, $"Initiate game over from invalid state: {state}, {playEnabled}");

        state = GameState.Inactive;
        dataService.SetState(GameState.Inactive);
        dataService.SetFinalScore(score);

        EventBus.Publish(new GameOverEvent { Data = dataService.GetGameData() });
        Reset();
    }

}
