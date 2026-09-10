using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;


public class ChooseAvatarView : PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Image avatarImage;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button chooseButton;

    // ==================================================
    // Private Fields
    // ==================================================
    private CellColor avatar;
    private RingList<CellColor> colorList;

    private ISpriteDatabase spriteDatabase;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(avatarImage != null, "[ChooseAvatarView] Null avatar image");

        Debug.Assert(leftButton != null, "[ChooseAvatarView] Null left button");
        Debug.Assert(rightButton != null, "[ChooseAvatarView] Null right button");
        Debug.Assert(chooseButton != null, "[ChooseAvatarView] Null choose button");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        spriteDatabase = ServiceLocator.Get<ISpriteDatabase>();

        List<CellColor> colors = Enum.GetValues(typeof(CellColor))
            .Cast<CellColor>()
            .Where(c => c != CellColor.Empty)
            .ToList();

        colorList = new RingList<CellColor>(colors);

        UIButtonFactory.Decrement<CellColor>(leftButton, colorList, SetAvatarSprite);
        UIButtonFactory.Increment<CellColor>(rightButton, colorList, SetAvatarSprite);
        UIButtonFactory.SendPatch(chooseButton, Host, () => new AvatarPatch(avatar));  // patch always needs to send the current value
    }

    protected override void SetInfo(IUIData data)
    {
        if (data is not ProfileData profile)
        {
            Debug.LogError($"[ChooseAvatarView] Data type mismatch, wanted ProfileData, recieved {data?.GetType().Name}");
            return;
        }

        SetAvatarSprite(profile.Avatar);
        colorList.SetCurrentIndexAtValue(profile.Avatar);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void SetAvatarSprite(CellColor color)
    {
        Debug.Assert(color != CellColor.Empty, "[ChooseAvatarView] Attempted to set avatar sprite to empty");

        avatarImage.sprite = spriteDatabase.GetSprite((int)color);
        avatar = color;
    }
}
