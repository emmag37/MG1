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

    private UIButton leftUIButton;
    private UIButton rightUIButton;
    private UIButton chooseUIButton;

    private ISpriteDatabase spriteDatabase;

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

        spriteDatabase = ServiceLocator.Get<ISpriteDatabase>();

        List<CellColor> colors = Enum.GetValues(typeof(CellColor))
            .Cast<CellColor>()
            .Where(c => c != CellColor.Empty)
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
        avatarImage.sprite = spriteDatabase.GetSprite(color);
        avatar = color;
    }
}
