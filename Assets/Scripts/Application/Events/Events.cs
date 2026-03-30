using UnityEngine;

// ==================================================
// Game Events
// ==================================================

// game state
public struct StartGameEvent { }
public struct ExitGameEvent { }
public struct GameOverEvent { }
public struct PauseGameEvent { }
public struct ResumeGameEvent { }

// update players
public struct SpawnPlayerEvent
{
    public CellColor Color;
}
public struct DestroyPlayerEvent { }

// update UI
public struct UpdateScoreEvent
{
    public int Score;
    public int HighScore;
}
public struct UpdatePlayerPreviewEvent
{
    public CellColor Color;
}


// ==================================================
// Board Events
// ==================================================

// game play - should declare this in the game manager?
public struct FullBoardEvent { }
public struct TurnCompletedEvent { }
public struct WinEvent
{
    public int Points;
}

// update player view
public struct ReturnPlayerEvent { }
public struct PlacePlayerEvent
{
    public Vector3 PlayerPosition;
}

// update board view
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

// update ghost preview
public struct GhostPreviewEvent
{
    public Vector2Int Index;
    public CellColor OriginalColor;
}

// ==================================================
// Player Events
// ==================================================

public struct InitializePlayerEvent
{
    public CellColor Color;
}

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