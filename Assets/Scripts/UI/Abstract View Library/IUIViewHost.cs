using UnityEngine;
using System;

// interface of functions that UI views themselves have access to

public interface IUIViewHost
{
    void PushView<TType>(TType type) where TType : struct, Enum;
    void PopView<TType>() where TType : struct, Enum;

    void PatchUpdate(IUIPatch patch);
}
