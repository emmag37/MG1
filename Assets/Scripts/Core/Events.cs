using UnityEngine;


// ==================================================
// Player Events
// ==================================================

public struct PlayerDraggingEvent       // used by: ghost preview
{
    public Transform PlayerTransform;
    public CellColor Color;
}

public struct PlayerReleasedEvent       // used by: board, ghost preview
{
    public Vector3 PlayerPosition;
    public CellColor Color;
}
