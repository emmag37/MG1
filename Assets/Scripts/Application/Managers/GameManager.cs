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
            Debug.Log("load an active game play");

            // TODO: DEBUG!!!
                // attempted but got very messed up
                // this branch runs, but the game play scene is empty including HUD?

            IGamePlayData game = dataService.GetGamePlayData();

            // load the current score and players
            score = game.CurrentScore;
            EventBus.Publish(new ScoreUpdateEvent { Score = score, HighScore = highScore });

            EventBus.Publish(new SpawnPlayerEvent { Color = game.CurrentPlayer, NextColor = game.NextPlayer });
            activePlayer = true;

            // load the board
            foreach (CellEntry cell in game.Board.Cells)
            {
                board.AddNonPlayer(new Vector2Int(cell.x, cell.y), (CellColor)cell.color);
            }
        }
        else
        {
            Debug.Log("fresh game");

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
        else if (state == GameState.Active && !playEnabled)
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
    private void HandleTurnCompleted(int points, (int, int) index)  // turn this into a local event
    {
        if (state == GameState.Tutorial) return;

        Debug.Assert(state == GameState.Active, $"Turn ran during invalid state: {state}");
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

        dataService.SaveTurn(score, index);

        RemoveCurrentPlayer();
        if (state == GameState.Active) SpawnNewPlayer();
    }

    private void HandleFullBoard()    // called on a game over  - turn this into a local event
    {
        Debug.Assert(state == GameState.Active && playEnabled, $"Initiate game over from invalid state: {state}, {playEnabled}");
        
        state = GameState.Inactive;
        dataService.SetState(GameState.Inactive);
        dataService.AddScore(score);

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

        dataService.ResetGame();
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

        if (state == GameState.Inactive) return;      // don't respawn on game over
        
        var playerColors = picker.CalculateNewPlayerColors();
        EventBus.Publish(new SpawnPlayerEvent { Color = playerColors.Color, NextColor = playerColors.NextColor });

        activePlayer = true;

        dataService.SavePlayerColors(playerColors.Color, playerColors.NextColor);
    }

    private void RemoveCurrentPlayer()
    {
        Debug.Assert(activePlayer, "Tried to remove when no active player");

        EventBus.Publish(new DestroyPlayerEvent());

        activePlayer = false;
    }
}
