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
    public int Row;
    public int Column;
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
    public CellColor OriginalColor;
}

// ==================================================
// Player Events
// ==================================================
/*
public struct InitializePlayerEvent
{
    public CellColor Color;
}*/


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
// Presentation Events
// ==================================================

public struct TransitionEvent { }
public struct UpdateSettingsEvent { }
