using UnityEngine;

public class TutorialController : MonoBehaviour
{
    // ==================================================
    // Private Fields
    // ==================================================

    private BoardController board;
    // private PlayerSpawner spawner;

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

    public void Initialize(BoardController board/*, PlayerSpawner spawner*/)
    {
        this.board = board;
        // this.spawner = spawner;

        currentStep = 0;
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void StartTutorial()
    {
        // run the first tutorial logic
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
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            default:
                break;
        }
    }
}
