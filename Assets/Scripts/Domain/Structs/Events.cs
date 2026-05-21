using UnityEngine;

// ==================================================
// Game Events
// ==================================================

// game state
public struct StartGameEvent
{
    public IGameData Data;
}
public struct ExitGameEvent { } 
public struct PauseGameEvent { }
public struct ResumeGameEvent { }
public struct GameOverEvent
{
    public IGameData Data;
}

public struct SpawnPlayerEvent
{
    public CellColor Color;
    public CellColor NextColor;
}
public struct DestroyPlayerEvent { }

public struct ScoreUpdateEvent
{
    public int Score;
    public int HighScore;
}


// ==================================================
// Board Events
// ==================================================

// game play
public struct WinEvent
{
    public Vector2Int Index;

    public bool Row;
    public bool Column;
    public bool RightDiag;
    public bool LeftDiag;

    public int Points;
}

// update player view
public struct ReturnPlayerEvent { }
public struct PlacePlayerEvent
{
    public Vector3 PlayerPosition;

    public Vector2Int Index;
    public CellColor Color;
}

// update ghost preview
public struct GhostPreviewEvent
{
    public Vector2Int Index;
    public bool On;
}

// continue tutorial
public struct TutorialStepCompleteEvent
{
    public int StepCompleted;   // indexed to 0
}

// ==================================================
// Player Events
// ==================================================

public struct PlayerDraggingEvent
{
    public Transform PlayerTransform;
    public CellColor Color;
}

public struct PlayerReleasedEvent
{
    public Vector3 PlayerPosition;  // turn this into index
    public CellColor Color;
}
