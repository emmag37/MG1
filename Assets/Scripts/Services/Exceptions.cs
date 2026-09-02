using UnityEngine;
using System;

public class GameLoadException : Exception
{
    public GameLoadException(string message) : base(message) { }
}
