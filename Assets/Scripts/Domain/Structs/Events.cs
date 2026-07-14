using UnityEngine;

// ==================================================
// Game Events
// ==================================================

// game state
public struct StartGameEvent
{
    public IGameData Data;
}

public struct GameOverEvent
{
    public IGameData Data;
}


// ==================================================
// Board Events
// ==================================================

// game play
public struct WinEvent { }  // only used by audio manager

// update player view
public struct PlacePlayerEvent { }  // used in audio manager and tutorial controller view

// continue tutorial
public struct TutorialStepCompleteEvent
{
    public int StepCompleted;   // indexed to 0
}

// ==================================================
// Player Events
// ==================================================

public struct PlayerDraggingEvent       // audio manager, ghost preview
{
    public Transform PlayerTransform;
    public CellColor Color;
}

public struct PlayerReleasedEvent       // board, ghost preview
{
    public Vector3 PlayerPosition;
    public CellColor Color;
}

public struct ScoreAnimationEvent { }   // only used in score animation
