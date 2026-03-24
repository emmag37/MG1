using UnityEngine;

// ==================================================
// Game Events
// ==================================================

public struct GameOverEvent { }
public struct UpdateScoreEvent { }
public struct UpdatePlayerPreviewEvent
{
    public CellColor Color;
}


// ==================================================
// UI Events
// ==================================================

public struct StartGameEvent { }
public struct EndGameEvent { }
public struct PauseGameEvent { }
public struct ResumeGameEvent { }
