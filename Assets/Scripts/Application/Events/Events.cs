using UnityEngine;



// ==================================================
// Game Events
// ==================================================

public struct UpdateScoreEvent
{
    public int Score;
    public int HighScore;
}
public struct UpdatePlayerPreviewEvent
{
    public CellColor Color;
}

public struct StartGameEvent { }
public struct ExitGameEvent { }
public struct GameOverEvent { }
public struct PauseGameEvent { }
public struct ResumeGameEvent { }


// ==================================================
// Board Events
// ==================================================

public struct FullBoardEvent { }
public struct TurnCompletedEvent { }
public struct WinEvent
{
    public int Points;
}

public struct PlayerOnBoardEvent
{
    public Vector2Int Index;
}

public struct ReturnPlayerEvent { }

public struct SetCellEvent
{
    public Vector2Int Index;
    public CellColor Color;
}
public struct ClearRowEvent
{
    public int Row;
}
public struct ClearColumnEvent
{
    public int Column;
}
public struct ClearRightDiagEvent { }
public struct ClearLeftDiagEvent { }

// this is only board view, so you need to clean up this logic
public struct PlacePlayerEvent
{
    public Vector3 PlayerPosition;
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

// ==================================================
// UI Events
// ==================================================

public struct TransitionEvent { }
public struct UpdateSettingsEvent { }