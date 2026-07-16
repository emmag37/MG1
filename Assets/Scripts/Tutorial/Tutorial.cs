using UnityEngine;
using System;
using System.Collections.Generic;

// eventually make the steps serializable to only load in when tutorial is actually used
public class Tutorial : MonoBehaviour
{
    // ==================================================
    // Private Fields
    // ==================================================

    private Board board;

    private int currentStep;
    private int turnsLeftInStep;    // makeshift recursion

    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize(Board board)
    {
        this.board = board;

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
        board.StartTutorialStep(playerColor, liveZone);
    }

    public void SkipTutorial()
    {
        List<CellEntry> cells = new List<CellEntry>();
        cells.Add(new CellEntry(0, 2, (int)CellColor.Color2));
        cells.Add(new CellEntry(1, 2, (int)CellColor.Color3));
        cells.Add(new CellEntry(2, 2, (int)CellColor.Color4));

        // set the board to the ending state - empty triggers board reset
        board.StartTutorialStep(CellColor.Empty, null, cells);
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    private void HandleStepComplete()
    {
        if (turnsLeftInStep == 0) // base case
        {
            EventBus.Publish(new TutorialStepCompleteEvent { StepCompleted = currentStep });
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

            cells.Add(new CellEntry(i, i, (int)CellColor.Color2));
            cells.Add(new CellEntry(i, 2, (int)CellColor.Color2));
            cells.Add(new CellEntry(GameConstants.RowSize - 1 - i, i, (int)CellColor.Color2));
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

        cells.Add(new CellEntry(0, 2, (int)CellColor.Color2));
        cells.Add(new CellEntry(1, 2, (int)CellColor.Color3));
        cells.Add(new CellEntry(2, 2, (int)CellColor.Color4));
        cells.Add(new CellEntry(3, 3, (int)CellColor.Color5));

        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (i == 3) continue;

            if (i != 2) cells.Add(new CellEntry(3, i, (int)CellColor.Color1));

            cells.Add(new CellEntry(4, i, (int)CellColor.Color6));
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
