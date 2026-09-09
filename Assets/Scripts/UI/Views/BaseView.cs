using UnityEngine;


public abstract class BaseView : UIView<BaseViewType>
{
    // ==================================================
    // Public Fields
    // ==================================================
    public override BaseViewType Type => baseType;

    // ==================================================
    // Inspector Fields
    // ==================================================

    [SerializeField] private BaseViewType baseType;
}
