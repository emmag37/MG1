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
        EventBus.Subscribe<TutorialStepCompleteEvent>(OnStepComplete);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<TutorialStepCompleteEvent>(OnStepComplete);
    }


    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize(BoardController board)
    {
        this.board = board;
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

    private void OnStepComplete(TutorialStepCompleteEvent e)
    {
        Debug.Assert(currentStep == e.StepCompleted, "Tutorial controller completed step mismatch");

        // for each case, start the logic for the next step in tutorial
        switch (e.StepCompleted)
        {
            case 0:
                // run step one
                break;
            case 1:
                // run step two
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
}
