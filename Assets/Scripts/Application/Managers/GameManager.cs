using UnityEngine;
using System;

/// <summary>
/// Controls the game states and initiates plays.
/// Bridges communication between player instances, the board, and UI updates.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ================================
    // Public Fields
    // ================================

    public bool ActiveGame => (state == GameState.Playing || state == GameState.Paused);    // remove - put in data service

    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private BoardController board;
    [SerializeField] private TutorialController tutorial;

    // ================================
    // Private Fields
    // ================================
    private GameDataService dataService;
    private PlayerPicker picker;

    private GameState state;
    private bool playEnabled;
    private bool activePlayer;

    private int score;
    private int highScore;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnDestroy()
    {
        board.FullBoard -= HandleFullBoard;
        board.TurnCompleted -= HandleTurnCompleted;
        tutorial.TutorialComplete -= HandleTutorialComplete;
    }


    // ================================
    // Initialize
    // ================================

    public void Initialize(GameDataService dataService)
    {
        this.dataService = dataService;

        picker = new PlayerPicker();
        board.Initialize();

        board.FullBoard += HandleFullBoard;
        board.TurnCompleted += HandleTurnCompleted;
        tutorial.TutorialComplete += HandleTutorialComplete;

        highScore = dataService.GetGameData().HighScore;
        state = dataService.GetGameData().State;

        playEnabled = true;
        if (state == GameState.Active)
        {
            // TODO: Load in previous game values
            activePlayer = true;
        }
        else
        {
            // initialize fresh values
            score = 0;
            activePlayer = false;
        }

    }


    // ================================
    // UI Commands
    // ================================

    // should tutorial be in first or second branch?
    // results in active game state with play enabled
    public void Play()
    {
        Debug.Assert(state != GameState.Active || !playEnabled, "Play called from active and enabled state"); // eventually going to be continue

        if (state == GameState.Inactive && !playEnabled)                                        // restart
        {
            Reset();
            NewGame();
        }
        else if (state == GameState.Inactive && playEnabled)   // fresh from home, game over
        {
            NewGame(); // can assume all game elements are reset
        }
        else if (state == GameState.Tutorial && playEnabled)
        {
            RunTutorial();
        }
        else
        {
            EventBus.Publish(new ResumeGameEvent());                                            // resume gameplay
        }

        playEnabled = true;
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
    private void HandleTurnCompleted(int points)  // turn this into a local event
    {
        if (state == GameState.Tutorial) return;

        Debug.Assert(state == GameState.Playing, $"Turn ran during invalid state: {state}");
        Debug.Assert(activePlayer, "Player turn completed but no active player");

        if (points > 0)
        {
            score += points;
            if (score > highScore)
            {
                highScore = score;
            }

            EventBus.Publish(new ScoreUpdateEvent { Score = score, HighScore = highScore });
        }

        RemoveCurrentPlayer();
        if (state == GameState.Playing) SpawnNewPlayer();

        // save the turn
    }

    private void HandleFullBoard()    // called on a game over  - turn this into a local event
    {
        Debug.Assert(state == GameState.Active && playEnabled, $"Initiate game over from invalid state: {state}, {playEnabled}");

        dataService.AddScore(score);
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
        playEnabled = false;
    }

    // ================================
    // Private Methods
    // ================================

    // game state helpers
    private void Reset()
    {
        Debug.Assert(state == GameState.Inactive, $"Reset called from state: {state}");

        RemoveCurrentPlayer();
        board.Reset();
        picker.Reset();

        score = 0;
        highScore = dataService.GetGameData().HighScore;
    }

    private void NewGame()
    {
        Debug.Assert(state == GameState.Inactive, $"New game called from state: {state}");

        state = GameState.Active;
        dataService.SetState(state);

        EventBus.Publish(new StartGameEvent { Data = dataService.GetGameData() });   // prepare systems not owned by the game manager
        SpawnNewPlayer();
    }

    public void RunTutorial()
    {
        Debug.Assert(state == GameState.Tutorial, $"Tutorial run from state: {state}");

        tutorial.Initialize(board);
        tutorial.StartTutorial();
    }


    // player helpers
    private void SpawnNewPlayer()
    {
        Debug.Assert(!activePlayer, "Tried to spawn while player active");

        if (state == GameState.Over) return;      // don't respawn on game over
        
        var playerColors = picker.CalculateNewPlayerColors();
        EventBus.Publish(new SpawnPlayerEvent { Color = playerColors.Color, NextColor = playerColors.NextColor });

        activePlayer = true;
    }

    private void RemoveCurrentPlayer()
    {
        Debug.Assert(activePlayer, "Tried to remove when no active player");

        EventBus.Publish(new DestroyPlayerEvent());

        activePlayer = false;
    }
}
