using UnityEngine;

public enum BaseViewType
{
    None,
    Home,
    GamePlay,
    GameOver,
    Tutorial
}

public abstract class BaseView : UIView<BaseViewType>
{
    public override BaseViewType Type => baseType;

    [SerializeField] private BaseViewType baseType;
}
