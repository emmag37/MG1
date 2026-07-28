using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public static class UIButtonFactory
{
    // i want to add a "transition" sound when this is base view type
    public static UIButton Navigate<TType>(Button button, IUIViewHost host, TType viewType) where TType : struct, Enum
        => new UIButton(button, () => host.PushView(viewType));

    public static UIButton ClosePopUp<TType>(Button button, IUIViewHost host) where TType : struct, Enum
        => new UIButton(button, host.PopView<TType>);

    public static UIButton EditInput(Button button, TMP_InputField inputField)
        => new UIButton(button, inputField.ActivateInputField);

    public static UIButton SendPatch(Button button, IUIViewHost host, Func<IUIPatch> getPatch)
        => new UIButton(button, () => host.PatchUpdate(getPatch()));

    public static UIButton Increment<T>(Button button, IIncrementList<T> list, Action<T> onChanged)
        => new UIButton(button, () => onChanged(list.Next()));

    public static UIButton Decrement<T>(Button button, IIncrementList<T> list, Action<T> onChanged)
        => new UIButton(button, () => onChanged(list.Prev()));

    // web link action eventually
}
