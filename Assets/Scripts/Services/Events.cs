using UnityEngine;


// ==================================================
// Player Events
// ==================================================

public struct PlayerDraggingEvent       // ghost preview
{
    public Transform PlayerTransform;
    public CellColor Color;
}

public struct PlayerReleasedEvent       // board, ghost preview
{
    public Vector3 PlayerPosition;
    public CellColor Color;
}
