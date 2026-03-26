using UnityEngine;



// ==================================================
// Game Events
// ==================================================

public struct UpdatePlayerPreviewEvent
{
    public CellColor Color;
}

public struct PlacePlayerEvent { }
public struct WinEvent { }
public struct GameOverEvent { }


// ==================================================
// Board Events
// ==================================================

public struct GameReadyEvent
{
    public Bounds BoardBounds;
}

// ==================================================
// UI Events
// ==================================================

public struct StartGameEvent { }
public struct EndGameEvent { }
public struct PauseGameEvent { }
public struct ResumeGameEvent { }

public struct TransitionEvent { }
public struct UpdateSettingsEvent { }