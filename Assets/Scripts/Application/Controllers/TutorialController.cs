using UnityEngine;

// bug with ghost preview - turns on once but then not again?
public class TutorialController : MonoBehaviour
{
    // ==================================================
    // Constants
    // ==================================================

    private const CellColor Color1 = CellColor.Color1;
    private const CellColor None = CellColor.Empty;

    // ==================================================
    // Private Fields
    // ==================================================

    private BoardController board;
    private int currentStep;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnEnable()
    {
        EventBus.Subscribe<ScoreAnimationEvent>(OnAnimationComplete);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<ScoreAnimationEvent>(OnAnimationComplete);
    }

    void OnDestroy()
    {
        board.TurnCompleted -= HandleTurnCompleted;
    }


    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize(BoardController board)
    {
        this.board = board;

        board.TurnCompleted += HandleTurnCompleted;
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void StartTutorial()
    {
        Debug.Log("running the tutorial");

        // step 0:
        currentStep = 0;
        EventBus.Publish(new SpawnPlayerEvent { Color = Color1, NextColor = None });
        board.SetLiveZone(new (int, int)[] { (2, 2) });
    }


    // ==================================================
    // Event Handlers
    // ==================================================

    // player placed on board
    private void HandleTurnCompleted(int points)
    {
        EventBus.Publish(new DestroyPlayerEvent());

        switch (currentStep)
        {
            case 0:
                StepOne();
                break;
            case 1:
                if (points == 0)
                    EventBus.Publish(new SpawnPlayerEvent { Color = Color1, NextColor = None });
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
        Debug.Log("animation complete");
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
                CompleteTutorial();
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

        (int, int)[] zone = new (int, int)[] { (2, 0), (2, 1), (2, 3), (2, 4) };
        board.SetLiveZone(zone);

        EventBus.Publish(new SpawnPlayerEvent { Color = Color1, NextColor = None });
    }

    private void StepTwo()
    {
        Debug.Assert(currentStep == 1);
        IncrementStep();

        // populate the scene
        board.SetLiveZone(null);

        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (i == 2) continue;

            board.AddNonPlayer(new Vector2Int(i, i), Color1); // no player, so hopefully no bug
            board.AddNonPlayer(new Vector2Int(i, 2), Color1);
            board.AddNonPlayer(new Vector2Int(GameConstants.RowSize - 1 - i, i), Color1);
        }

        board.SetLiveZone(new (int, int)[] { (2, 2) });
        EventBus.Publish(new SpawnPlayerEvent { Color = Color1, NextColor = None });
    }

    private void StepThree()
    {
        Debug.Assert(currentStep == 2);
        IncrementStep();

        // populate the scene - did not populate correctly
        board.SetLiveZone(null);

        board.AddNonPlayer(new Vector2Int(0, 2), CellColor.Color2);
        board.AddNonPlayer(new Vector2Int(1, 2), CellColor.Color3);
        board.AddNonPlayer(new Vector2Int(2, 2), CellColor.Color4);

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

        board.SetLiveZone(new (int, int)[] { (3, 2) });
        EventBus.Publish(new SpawnPlayerEvent { Color = Color1, NextColor = None });
    }

    private void StepFour()
    {
        Debug.Assert(currentStep == 3);
        IncrementStep();

        board.SetLiveZone(new (int, int)[] { (4, 3) });
        EventBus.Publish(new SpawnPlayerEvent { Color = CellColor.WildCard, NextColor = None });
    }

    private void StepFive()
    {
        Debug.Assert(currentStep == 4);
        IncrementStep();

        board.SetLiveZone(new (int, int)[] { (3, 3) });
        EventBus.Publish(new SpawnPlayerEvent { Color = CellColor.Mask, NextColor = None });
    }

    private void CompleteTutorial()
    {
        Debug.Assert(currentStep == 5);
        IncrementStep();

        board.SetLiveZone(null);
    }

    // helper
    private void IncrementStep()
    {
        EventBus.Publish(new TutorialStepCompleteEvent { StepCompleted = currentStep });
        currentStep++;
    }

}
