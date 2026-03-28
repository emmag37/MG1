using UnityEngine;



// ==================================================
// Game Events
// ==================================================

public struct UpdateScoreEvent
{
    public int Score;
}
public struct UpdatePlayerPreviewEvent
{
    public CellColor Color;
}

public struct StartGameEvent { }
public struct ExitGameEvent { }
public struct GameOverEvent { }


// ==================================================
// Board Events
// ==================================================

public struct FullBoardEvent { }

public struct PlacePlayerEvent
{
    public Vector3 PlayerPosition;
}
public struct ReturnPlayerEvent { }

public struct TurnCompletedEvent { }
public struct WinEvent
{
    public int Points;
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
    public Vector3 PlayerPosition;
    public CellColor Color;
}

// ==================================================
// UI Events
// ==================================================


public struct PauseGameEvent { }
public struct ResumeGameEvent { }

public struct TransitionEvent { }
public struct UpdateSettingsEvent { }