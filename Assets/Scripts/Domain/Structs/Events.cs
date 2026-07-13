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
public struct WinEvent { }

// update player view
public struct PlacePlayerEvent { }

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

public struct ScoreAnimationEvent { }
