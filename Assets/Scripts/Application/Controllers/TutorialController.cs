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

    // need to subscribe
    private void HandleTurnCompleted(int points)
    {
        EventBus.Publish(new DestroyPlayerEvent());

        switch (currentStep)
        {
            case 0:
                StepOne();
                break;
            case 1:
                if (points > 0)
                    StepTwo();
                else
                    EventBus.Publish(new SpawnPlayerEvent { Color = Color1, NextColor = None });
                break;
            case 2:
                // run step three
                break;
            case 3:
                // run step four
                break;
            case 4:
                // run step five
                break;
            case 5:
                // run step six
                break;
            default:
                // set the live zone back to the entire board
                break;
        }
    }


    // ==================================================
    // Private Functions
    // ==================================================

    private void StepOne()
    {
        Debug.Assert(currentStep == 0);

        EventBus.Publish(new TutorialStepCompleteEvent { StepCompleted = currentStep });
        currentStep++;

        (int, int)[] zone = new (int, int)[] { (2, 0), (2, 1), (2, 3), (2, 4) };
        board.SetLiveZone(zone);

        EventBus.Publish(new SpawnPlayerEvent { Color = Color1, NextColor = None });
    }

    private void StepTwo()
    {
        Debug.Assert(currentStep == 1);

        EventBus.Publish(new TutorialStepCompleteEvent { StepCompleted = currentStep });
        currentStep++;

        Debug.Log("populate the scene");
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (i == 2) continue;

            // figure out which function to use
            //EventBus.Publish(new PlayerReleasedEvent { Color = Color1, Index = { i, i }, PlayerPosition = Vector3.zero });

           // (i, i)
           // (i, 2)
           // (GameConstants.RowSize - i, i)
        }

        board.SetLiveZone(new (int, int)[] { (2, 2) });

        EventBus.Publish(new SpawnPlayerEvent { Color = Color1, NextColor = None });
    }

}
