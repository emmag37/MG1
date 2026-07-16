using UnityEngine;
using System;

[Serializable]
public struct CellEntry
{
    public int x, y, color;
    public CellEntry(int x, int y, int color) { this.x = x; this.y = y; this.color = color; }
}
