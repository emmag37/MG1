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

            NewGame();
        }
        else if (state == GameState.Inactive && playEnabled)   // fresh from home, game over
        {
            Debug.Log("fresh game branch");
            NewGame(); // can assume all game elements are reset
        }
        else if (state == GameState.Active && !playEnabled)
        {
            playEnabled = true;
            board.PauseGame(false);                                            // resume gameplay
        }
        else if (state == GameState.Active && playEnabled)  // old game not loaded flag
        {
            // used to load game here

            board.FreshGame();
            EventBus.Publish(new StartGameEvent());  // continue the gameplay
        }
        else
        {
            Debug.Log("continue game");
            playEnabled = true;
        }
    }

    // pause == true to pause, pause == false to unpause
    public void Pause(bool pause)
    {
        Debug.Assert(state == GameState.Active, $"Pause called with non-active state, state = {state}");

        playEnabled = !pause;
        board.PauseGame(pause);
    }

    public void Restart()
    {
        // only need to change states
        state = GameState.Inactive;
        dataService.SetState(GameState.Inactive);

        Play();
    }

    public void StartTutorial()
    {
        Debug.Assert(state == GameState.Tutorial, $"Tutorial run from state: {state}");

        //tutorial.Initialize(board);
        tutorial.StartTutorial();
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

    private void NewGame()
    {
        Debug.Assert(state == GameState.Inactive, $"New game called from state: {state}");

        Debug.Log("new game!!!");

        state = GameState.Active;
        dataService.SetState(state);

        board.FreshGame();
        EventBus.Publish(new StartGameEvent());   // prepare systems not owned by the game manager
    }

}
