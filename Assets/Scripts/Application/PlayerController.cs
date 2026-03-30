using UnityEngine;
using System;

// want to remove the monobehavior from this file
    // doesn't seem worth it for now
    // wait until after you complete the start drag property
public class PlayerController : MonoBehaviour
{
    // ================================
    // Public Methods
    // ================================

    private CellColor color;
    // can add other player states to help with debugging later as well

    // ================================
    // Public Methods
    // ================================

    public void Initialize(CellColor color)
    {
        this.color = color;
    }

    public void StartDrag()
    {
        EventBus.Publish(new PlayerDraggingEvent { Color = color });
    }

    public void Released(Vector3 position)
    {
        EventBus.Publish(new PlayerReleasedEvent { PlayerPosition = position, Color = color });
    }
}
