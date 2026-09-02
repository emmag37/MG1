using UnityEngine;
using System;
using System.Collections.Generic;

// eventually make the steps serializable to only load in when tutorial is actually used
    // data-driven
[RequireComponent(typeof(TutorialViewController))]
public class Tutorial : MonoBehaviour
{
    // ==================================================
    // Private Fields
    // ==================================================
    private Board board;
    private TutorialViewController viewController;

    private int currentStep;
    private int turnsLeftInStep;    // makeshift recursion

    // ==================================================
    // Initializer
    // ==================================================
    public void Initialize(Board board)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        this.board = board;

        viewController = GetComponent<TutorialViewController>();

        currentStep = 0;
        turnsLeftInStep = 0;

        board.TutorialStepComplete += HandleStepComplete;
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public void StartTutorial()
    {
        // prepare step 0 data
        (int, int)[] liveZone = { (2, 2) };
        CellColor playerColor = CellColor.Color1;

        // run step in board
        board.StartTutorialStep(playerColor, liveZone);     // check this
    }

    public void SkipTutorial()
    {
        List<CellEntry> cells = new List<CellEntry>();
        cells.Add(new CellEntry(0, 2, CellColor.Color2));
        cells.Add(new CellEntry(1, 2, CellColor.Color3));
        cells.Add(new CellEntry(2, 2, CellColor.Color4));

        board.StartTutorialStep(CellColor.Empty, null, cells);      // set the board to the ending state - empty triggers board reset
        viewController.SkipTutorial();
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    private void HandleStepComplete(Vector2Int index)
    {
        viewController.RemoveArrow(index);

        if (turnsLeftInStep == 0) // base case
        {
            viewController.ShowNextStep(currentStep);
            currentStep++;
        }

        switch (currentStep)
        {
            case 1:
                StepOne();
                break;
            case 2:
                StepTwo();
                break;
            case 3:
                StepThree();
                break;
            case 4:
                StepFour();
                break;
            case 5:
                StepFive();
                break;
            default:
                break;
        }
    }


    // ==================================================
    // Private Functions
    // ==================================================

    private void StepOne()
    {
        if (turnsLeftInStep == 0)   // initialize step turns/spawns
            turnsLeftInStep = 4;

        turnsLeftInStep--;

        // prepare step one data
        CellColor playerColor = CellColor.Color1;
        (int, int)[] liveZone = { (2, 0), (2, 1), (2, 3), (2, 4) };

        board.StartTutorialStep(playerColor, liveZone);     // runs recursive step - event will call back
    }

    private void StepTwo()
    {
        // prepare step two data
        CellColor playerColor = CellColor.Color2;
        (int, int)[] liveZone = { (2, 2) };

        // create step 2 cell list
        List<CellEntry> cells = new List<CellEntry>();
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (i == 2) continue;

            cells.Add(new CellEntry(i, i, CellColor.Color2));
            cells.Add(new CellEntry(i, 2, CellColor.Color2));
            cells.Add(new CellEntry(GameConstants.RowSize - 1 - i, i, CellColor.Color2));
        }

        board.StartTutorialStep(playerColor, liveZone, cells);
    }

    private void StepThree()
    {
        // prepare step three data
        CellColor playerColor = CellColor.Color1;
        (int, int)[] liveZone = { (3, 2) };

        // create cell list
        List<CellEntry> cells = new List<CellEntry>();

        cells.Add(new CellEntry(0, 2, CellColor.Color2));
        cells.Add(new CellEntry(1, 2, CellColor.Color3));
        cells.Add(new CellEntry(2, 2, CellColor.Color4));
        cells.Add(new CellEntry(3, 3, CellColor.Color5));

        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (i == 3) continue;

            if (i != 2) cells.Add(new CellEntry(3, i, CellColor.Color1));

            cells.Add(new CellEntry(4, i, CellColor.Color6));
        }

        board.StartTutorialStep(playerColor, liveZone, cells);
    }

    private void StepFour()
    {
        CellColor playerColor = CellColor.WildCard;
        (int, int)[] liveZone = { (4, 3) };

        board.StartTutorialStep(playerColor, liveZone);
    }

    private void StepFive()
    {
        CellColor playerColor = CellColor.Mask;
        (int, int)[] liveZone = { (3, 3) };

        board.StartTutorialStep(playerColor, liveZone);
    }
}
