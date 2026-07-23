using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

// todo: reimplement update avatar
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

    private UIButton leftUIButton;
    private UIButton rightUIButton;
    private UIButton chooseUIButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(avatarImage != null, "Avatar image not set in choose avatar view");

        Debug.Assert(leftButton != null, "Left button not set in choose avatar view");
        Debug.Assert(rightButton != null, "Right button not set in choose avatar view");
        Debug.Assert(chooseButton != null, "Choose button not set in choose avatar view");
    }

    protected override void Awake()
    {
        base.Awake();

        List<CellColor> colors = Enum.GetValues(typeof(CellColor))
            .Cast<CellColor>()
            .Where(c => c != CellColor.Empty && c != CellColor.ResetShadow && c != CellColor.Shadow)
            .ToList();

        colorList = new RingList<CellColor>(colors);

        leftUIButton = UIButtonFactory.Decrement<CellColor>(leftButton, colorList, SetAvatarSprite);
        rightUIButton = UIButtonFactory.Increment<CellColor>(rightButton, colorList, SetAvatarSprite);
        chooseUIButton = UIButtonFactory.SendPatch(chooseButton, Host, () => new AvatarPatch(avatar));  // patch always needs to send the current value
    }

    // ==================================================
    // Base Class Methods
    // ==================================================

    protected override void SetInfo(IUIData data)
    {
        if (data is not ProfileData profile)
        {
            Debug.LogError($"Data type mismatch, wanted ProfileData, recieved {data?.GetType().Name}");
            return;
        }

        SetAvatarSprite(profile.Avatar);
        colorList.SetCurrentValue(profile.Avatar);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void SetAvatarSprite(CellColor color)
    {
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite(color);
        avatar = color;
    }
}
