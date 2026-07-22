using UnityEngine;

// ==================================================
// Board Events
// ==================================================

// update player view
public struct PlacePlayerEvent { public Vector2Int Index; }  // tutorial controller view


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
