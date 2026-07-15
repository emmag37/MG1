using UnityEngine;
using System;

// bug with ghost preview - turns on once but then not again?
public class TutorialController : MonoBehaviour
{
    
    // ==================================================
    // Constants
    // ==================================================

    private const CellColor Color1 = CellColor.Color1;
    private const CellColor None = CellColor.Empty;

    // ==================================================
    // Events
    // ==================================================

    //public event Action TutorialComplete;

    // ==================================================
    // Private Fields
    // ==================================================

    //private BoardController board;
    //private int currentStep;

    //private bool activePlayer = false;
    //private bool activeZone = false;    // true when the live areas of the board have changed

    // ==================================================
    // Unity Lifecycle
    // =================================================

    void OnEnable()
    {
        //EventBus.Subscribe<ScoreAnimationEvent>(OnAnimationComplete);
    }

    void OnDisable()
    {
        //EventBus.Unsubscribe<ScoreAnimationEvent>(OnAnimationComplete);
    }

    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize()
    {
        //this.board = board;
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void StartTutorial()
    {
        /*
        // step 0:
        currentStep = 0;
        SpawnPlayer(Color1);
        SetLiveZone(new (int, int)[] { (2, 2) });

        board.TurnCompleted += HandleTurnCompleted;
        */
    }

    public void CompleteTutorial()
    {
        /*
        // reset and clear the play space
        if (activePlayer) DestroyPlayer();
        if (activeZone) ResetZone();
        board.TurnCompleted -= HandleTurnCompleted;
        board.Reset(); */

        //TutorialComplete?.Invoke();

        //Debug.Assert(!activePlayer && !activeZone, "Tutorial not properly reset");
        
    }


    // ==================================================
    // Event Handlers
    // ==================================================
    /*
    // player placed on board
    private void HandleTurnCompleted(int points, (int, int) index)
    {
        DestroyPlayer();

        switch (currentStep)
        {
            case 0:
                StepOne();
                break;
            case 1:
                if (points == 0)
                    SpawnPlayer(Color1);
                break;
            case 3:
                StepFour();
                break;
            default:
                // called when the players for case 2, 4, 5 are placed
                break;
        }
    }

    // win sequence completed
    private void OnAnimationComplete(ScoreAnimationEvent e)
    {
        switch (currentStep)
        {
            case 1:
                StepTwo();
                break;
            case 2:
                StepThree();
                break;
            case 4:
                StepFive();
                break;
            case 5:
                StepSeven();
                break;
            default:
                // error with case 0 or 3, should never be called
                break;
        }
    }


    // ==================================================
    // Private Functions
    // ==================================================

    private void StepOne()
    {
        Debug.Assert(currentStep == 0);
        IncrementStep();

        SetLiveZone(new (int, int)[] { (2, 0), (2, 1), (2, 3), (2, 4) });
        SpawnPlayer(Color1);
    }

    private void StepTwo()
    {
        Debug.Assert(currentStep == 1);
        IncrementStep();

        // populate the scene
        ResetZone();

        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (i == 2) continue;

            board.AddNonPlayer(new Vector2Int(i, i), Color1); // no player, so hopefully no bug
            board.AddNonPlayer(new Vector2Int(i, 2), Color1);
            board.AddNonPlayer(new Vector2Int(GameConstants.RowSize - 1 - i, i), Color1);
        }

        SetLiveZone(new (int, int)[] { (2, 2) });
        SpawnPlayer(Color1);
    }

    private void StepThree()
    {
        Debug.Assert(currentStep == 2);
        IncrementStep();

        // populate the scene
        ResetZone();

        PopulateExtraPieces();
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (i == 3)
            {
                board.AddNonPlayer(new Vector2Int(3, i), CellColor.Color5); // this is not being added
                continue;
            }

            board.AddNonPlayer(new Vector2Int(4, i), CellColor.Color6);
            if (i != 2) board.AddNonPlayer(new Vector2Int(3, i), Color1);
        }

        SetLiveZone(new (int, int)[] { (3, 2) });
        SpawnPlayer(Color1);
    }

    private void StepFour()
    {
        Debug.Assert(currentStep == 3);
        IncrementStep();

        SetLiveZone(new (int, int)[] { (4, 3) });
        SpawnPlayer(CellColor.WildCard);
    }

    private void StepFive()
    {
        Debug.Assert(currentStep == 4);
        IncrementStep();

        SetLiveZone(new (int, int)[] { (3, 3) });
        SpawnPlayer(CellColor.Mask);
    }

    private void StepSeven()
    {
        Debug.Assert(currentStep == 5);
        IncrementStep();

        CompleteTutorial();
    }

    // ==================================================
    // Private Helper Functions
    // ==================================================

    private void IncrementStep()
    {
        EventBus.Publish(new TutorialStepCompleteEvent { StepCompleted = currentStep });
        currentStep++;
    }

    private void PopulateExtraPieces()
    {
        board.AddNonPlayer(new Vector2Int(0, 2), CellColor.Color2);
        board.AddNonPlayer(new Vector2Int(1, 2), CellColor.Color3);
        board.AddNonPlayer(new Vector2Int(2, 2), CellColor.Color4);
    }

    private void SpawnPlayer(CellColor color)
    {
        Debug.Assert(!activePlayer, "Attempted to spawn an additional player in tutorial");

        EventBus.Publish(new SpawnPlayerEvent { Color = color, NextColor = None });
        activePlayer = true;
    }

    private void DestroyPlayer()
    {
        Debug.Assert(activePlayer, "Attempted to destroy non-existent player in tutorial");

        EventBus.Publish(new DestroyPlayerEvent());
        activePlayer = false;
    }

    private void SetLiveZone((int, int)[] zone)
    {
        board.SetLiveZone(zone);
        activeZone = true;
    }

    private void ResetZone()
    {
        board.SetLiveZone(null);
        activeZone = false;
    }
    */
}
