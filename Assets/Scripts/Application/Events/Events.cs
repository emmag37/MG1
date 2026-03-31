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

// update players - make this local whenever you update game initialization
public struct SpawnPlayerEvent
{
    public CellColor Color;
}
public struct DestroyPlayerEvent { }


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
    public CellColor OriginalColor;
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
// Presentation Events
// ==================================================

// potentially make these local as well - i don't think any of these should be global
public struct TransitionEvent { }
public struct UpdateSettingsEvent { }
